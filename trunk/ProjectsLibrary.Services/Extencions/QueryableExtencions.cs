using Microsoft.EntityFrameworkCore;
using ProjectsLibrary.Domain.Exceptions;
using ProjectsLibrary.Domain.Models.Enums;
using ProjectsLibrary.Domain.Models.RequestModels;
using System.Linq.Expressions;
using System.Reflection;

namespace ProjectsLibrary.Services.Extencions
{
    public static class QueryableExtencions
    {
        public static IQueryable<T> Sort<T>(this IQueryable<T> source, SortParams sortParams)
        {
            if (sortParams == null)
                return source;

            var keySelector = GetPropertyExpressionOrDefault<T>(sortParams.OrderByParam);

            string methodName = sortParams.Direction == SortDirection.Ascending ? "OrderBy" : "OrderByDescending";

            var parameter = Expression.Parameter(typeof(T), "x");
            var body = Expression.PropertyOrField(parameter, ((PropertyInfo)((MemberExpression)keySelector.Body).Member).Name);
            var lambda = Expression.Lambda(body, parameter);

            var methodCallExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), body.Type },
                source.Expression,
                Expression.Quote(lambda)
            );

            return source.Provider.CreateQuery<T>(methodCallExpression);
        }

        public static LambdaExpression GetPropertyExpressionOrDefault<T>(string propertyName)
        {
            var param = Expression.Parameter(typeof(T), "x");
            PropertyInfo? property = null;

            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                property = typeof(T).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                                    .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));
            }

            if (property == null)
            {
                property = typeof(T).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                                    .FirstOrDefault(p => string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase));
            }

            if (property == null)
            {
                property = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();
            }

            if (property == null)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have any public properties.");
            }

            var body = Expression.Property(param, property);
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), property.PropertyType);
            return Expression.Lambda(delegateType, body, param);
        }

        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, PageParams pageParams)
        {
            if (pageParams == null)
            {
                return query;
            }

            var skip = Math.Max(0, pageParams.Skip);
            var take = pageParams.PageSize > 0 ? pageParams.PageSize : 10;

            return query.Skip(skip).Take(take);
        }

        public static IQueryable<T> Filter<T>(this IQueryable<T> query, FilterParams filterParams)
        {
            if (string.IsNullOrWhiteSpace(filterParams.SearchingValue) || filterParams.SearchableFieldsNames.Length == 0)
                return query;

            var parameter = Expression.Parameter(typeof(T), "e");

            var efFunctionsProperty = Expression.Property(null, typeof(EF).GetProperty(nameof(EF.Functions)));

            var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
                "Like",
                new[] { typeof(DbFunctions), typeof(string), typeof(string) }
            );

            var pattern = "%" + filterParams.SearchingValue + "%";
            var patternExpression = Expression.Constant(pattern);

            Expression? combinedPredicate = null;

            foreach (var fieldName in filterParams.SearchableFieldsNames)
            {
                if (string.IsNullOrWhiteSpace(fieldName))
                    throw new NullOrEmptyFieldNameExсeption(message: "The request contains null or empty field name for search");

                var propertyInfo = typeof(T).GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (propertyInfo == null)
                    throw new PropertyNotFoundException(message: $"Property with name {fieldName} doesn`t exists");

                var propertyExpression = Expression.Property(parameter, propertyInfo);

                Expression propertyAsStringExpression = BuildPropertyAsStringExpression(propertyExpression);

                var likeCall = Expression.Call(
                    null,
                    likeMethod,
                    efFunctionsProperty,
                    propertyAsStringExpression,
                    patternExpression);

                combinedPredicate = combinedPredicate == null
                    ? likeCall
                    : Expression.OrElse(combinedPredicate, likeCall);
            }

            if (combinedPredicate == null)
                return query; 

            var lambda = Expression.Lambda<Func<T, bool>>(combinedPredicate, parameter);

            return query.Where(lambda);
        }

        private static Expression BuildPropertyAsStringExpression(Expression propertyExpression)
        {
            if (propertyExpression.Type == typeof(string))
            {
                var nullCheck = Expression.NotEqual(propertyExpression, Expression.Constant(null, typeof(string)));
                return Expression.Condition(nullCheck, propertyExpression, Expression.Constant(string.Empty));
            }

            var underlyingType = Nullable.GetUnderlyingType(propertyExpression.Type);
            if (underlyingType != null)
            {
                var hasValue = Expression.Property(propertyExpression, "HasValue");
                var value = Expression.Property(propertyExpression, "Value");
                var toStringMethod = underlyingType.GetMethod("ToString", Type.EmptyTypes)!;
                var toStringCall = Expression.Call(value, toStringMethod);

                return Expression.Condition(hasValue, toStringCall, Expression.Constant(string.Empty));
            }

            var toString = propertyExpression.Type.GetMethod("ToString", Type.EmptyTypes);
            if (toString == null)
                throw new InvalidOperationException($"Type {propertyExpression.Type} does not have a ToString method");

            if (!propertyExpression.Type.IsValueType)
            {
                var nullCheck = Expression.NotEqual(propertyExpression, Expression.Constant(null, propertyExpression.Type));
                var toStringCall = Expression.Call(propertyExpression, toString);
                return Expression.Condition(nullCheck, toStringCall, Expression.Constant(string.Empty));
            }
            else
            {
                return Expression.Call(propertyExpression, toString);
            }
        }
    }
}
