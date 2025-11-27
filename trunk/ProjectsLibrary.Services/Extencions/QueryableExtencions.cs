using Microsoft.EntityFrameworkCore;
using ProjectsLibrary.Domain.Models.Enums;
using ProjectsLibrary.Domain.Models.RequestModels;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace ProjectsLibrary.Services.Extencions {
    public static class QueryableExtensions {
        private static readonly ConcurrentDictionary<string, LambdaExpression> _sortExpressionCache = new();
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();
        private static readonly ConcurrentDictionary<string, MethodInfo> _likeMethodCache = new();

        public static IQueryable<T> Sort<T>(this IQueryable<T> source, SortParams? sortParams) {
            if (sortParams == null)
                return source;

            var keySelector = GetPropertyExpressionCached<T>(sortParams.OrderByParam);

            string methodName = sortParams.Direction == SortDirection.Ascending ? "OrderBy" : "OrderByDescending";

            var methodCallExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), keySelector.ReturnType },
                source.Expression,
                Expression.Quote(keySelector)
            );

            return source.Provider.CreateQuery<T>(methodCallExpression);
        }

        private static LambdaExpression GetPropertyExpressionCached<T>(string? propertyName) {
            var cacheKey = $"{typeof(T).FullName}_{propertyName?.ToLowerInvariant() ?? "default"}";

            return _sortExpressionCache.GetOrAdd(cacheKey, _ => BuildPropertyExpression<T>(propertyName));
        }

        private static LambdaExpression BuildPropertyExpression<T>(string? propertyName) {
            var param = Expression.Parameter(typeof(T), "x");
            PropertyInfo property = FindProperty<T>(propertyName);

            var body = Expression.Property(param, property);
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), property.PropertyType);

            return Expression.Lambda(delegateType, body, param);
        }

        private static PropertyInfo FindProperty<T>(string? propertyName) {
            var properties = GetCachedProperties<T>();

            PropertyInfo? property = null;

            if (!string.IsNullOrWhiteSpace(propertyName)) {
                property = properties.FirstOrDefault(p =>
                    string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));
            }

            property ??= properties.FirstOrDefault(p =>
                string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase));

            property ??= properties.FirstOrDefault();

            if (property == null) {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have any public properties.");
            }

            return property;
        }

        private static PropertyInfo[] GetCachedProperties<T>() {
            var type = typeof(T);
            return _propertyCache.GetOrAdd(type, t =>
                t.GetProperties(BindingFlags.Public | BindingFlags.Instance));
        }

        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, PageParams? pageParams) {
            if (pageParams == null) {
                return query;
            }

            var skip = Math.Max(0, pageParams.Skip);
            var take = pageParams.PageSize > 0 ? pageParams.PageSize : 10;

            return query.Skip(skip).Take(take);
        }

        public static IQueryable<T> Filter<T>(this IQueryable<T> query, FilterParams? filterParams) {
            if (filterParams == null ||
                string.IsNullOrWhiteSpace(filterParams.SearchingValue) ||
                filterParams.SearchableFieldsNames == null ||
                filterParams.SearchableFieldsNames.Length == 0) {
                return query;
            }

            var parameter = Expression.Parameter(typeof(T), "e");
            Expression? combinedPredicate = BuildSearchPredicate<T>(parameter, filterParams);

            if (combinedPredicate == null)
                return query;

            var lambda = Expression.Lambda<Func<T, bool>>(combinedPredicate, parameter);
            return query.Where(lambda);
        }

        private static Expression? BuildSearchPredicate<T>(ParameterExpression parameter, FilterParams filterParams) {
            var likeMethod = GetLikeMethod();
            var efFunctionsProperty = Expression.Property(null, typeof(EF).GetProperty(nameof(EF.Functions))!);
            var pattern = "%" + filterParams.SearchingValue + "%";
            var patternExpression = Expression.Constant(pattern);

            Expression? combinedPredicate = null;

            foreach (var fieldName in filterParams.SearchableFieldsNames) {
                if (string.IsNullOrWhiteSpace(fieldName))
                    throw new ArgumentException("The request contains null or empty field name for search");

                var propertyExpression = BuildPropertyAccessExpression<T>(parameter, fieldName);
                var stringExpression = BuildPropertyAsStringExpression(propertyExpression);

                var likeCall = Expression.Call(
                    null,
                    likeMethod,
                    efFunctionsProperty,
                    stringExpression,
                    patternExpression);

                combinedPredicate = combinedPredicate == null
                    ? likeCall
                    : Expression.OrElse(combinedPredicate, likeCall);
            }

            return combinedPredicate;
        }

        private static MemberExpression BuildPropertyAccessExpression<T>(ParameterExpression parameter, string fieldName) {
            var properties = GetCachedProperties<T>();
            var propertyInfo = properties.FirstOrDefault(p =>
                string.Equals(p.Name, fieldName, StringComparison.OrdinalIgnoreCase));

            if (propertyInfo == null)
                throw new InvalidOperationException($"Property with name {fieldName} doesn't exist in type {typeof(T).Name}");

            return Expression.Property(parameter, propertyInfo);
        }

        private static MethodInfo GetLikeMethod() {
            return _likeMethodCache.GetOrAdd("Like", _ =>
                typeof(DbFunctionsExtensions).GetMethod(
                    "Like",
                    new[] { typeof(DbFunctions), typeof(string), typeof(string) }
                ) ?? throw new InvalidOperationException("Like method not found")
            );
        }

        private static Expression BuildPropertyAsStringExpression(MemberExpression propertyExpression) {
            var propertyType = propertyExpression.Type;

            if (propertyType == typeof(string)) {
                return HandleStringType(propertyExpression);
            }

            var underlyingType = Nullable.GetUnderlyingType(propertyType);
            if (underlyingType != null) {
                return HandleNullableType(propertyExpression, underlyingType);
            }

            return HandleValueType(propertyExpression, propertyType);
        }

        private static Expression HandleStringType(MemberExpression propertyExpression) {
            var nullCheck = Expression.NotEqual(propertyExpression, Expression.Constant(null, typeof(string)));
            return Expression.Condition(nullCheck, propertyExpression, Expression.Constant(string.Empty));
        }

        private static Expression HandleNullableType(MemberExpression propertyExpression, Type underlyingType) {
            var hasValue = Expression.Property(propertyExpression, "HasValue");
            var value = Expression.Property(propertyExpression, "Value");
            var toStringMethod = underlyingType.GetMethod("ToString", Type.EmptyTypes);

            if (toStringMethod == null)
                return Expression.Constant(string.Empty);

            var toStringCall = Expression.Call(value, toStringMethod);
            return Expression.Condition(hasValue, toStringCall, Expression.Constant(string.Empty));
        }

        private static Expression HandleValueType(MemberExpression propertyExpression, Type propertyType) {
            var toStringMethod = propertyType.GetMethod("ToString", Type.EmptyTypes);
            if (toStringMethod == null)
                return Expression.Constant(string.Empty);

            if (!propertyType.IsValueType) {
                var nullCheck = Expression.NotEqual(propertyExpression, Expression.Constant(null, propertyType));
                var toStringCall = Expression.Call(propertyExpression, toStringMethod);
                return Expression.Condition(nullCheck, toStringCall, Expression.Constant(string.Empty));
            } else {
                return Expression.Call(propertyExpression, toStringMethod);
            }
        }
    }
}
