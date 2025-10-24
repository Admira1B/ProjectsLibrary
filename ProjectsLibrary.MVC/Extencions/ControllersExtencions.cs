using ProjectsLibrary.Domain.Exceptions;
using ProjectsLibrary.Domain.Models.Enums;
using ProjectsLibrary.Domain.Models.RequestModels;
using System.Security.Claims;

namespace ProjectsLibrary.API.Extencions
{
    public static class ControllersExtencions
    {
        public static (FilterParams filterParams, SortParams sortParams, PageParams pageParams) BuildGetMethodModelParams(GetPagedModel model) 
        {
            var filterParams = new FilterParams()
            {
                SearchingValue = model.SearchingValue,
                SearchableFieldsNames = model.SearchableFieldsNames
            };

            var sortParams = new SortParams()
            {
                Direction = model.SortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending,
                OrderByParam = model.SortColumn
            };

            var pageParams = new PageParams()
            {
                PageSize = model.Length,
                Skip = model.Start,
            };

            return (filterParams, sortParams, pageParams);
        }

        public static EmployeeRole GetUserRole(ClaimsPrincipal user)
        {
            var roleString = user.FindFirst("employeeRole")?.Value ?? null;
            if (roleString == null)
                throw new NoRoleClaimToCurrentUserException(message: "Cannot found claim with user role information");

            var role = EmployeeRoleExtension.StringToRole(roleString);
            if (role == null)
                throw new UnknownUserRoleException(message: $"{roleString} is unknown role");

            return (EmployeeRole)role;
        }
    }
}
