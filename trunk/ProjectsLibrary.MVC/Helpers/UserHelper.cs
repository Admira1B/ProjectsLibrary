using ProjectsLibrary.Domain.Exceptions;
using ProjectsLibrary.Domain.Models.Enums;
using System.Security.Claims;

namespace ProjectsLibrary.MVC.Helpers {
    public static class UserHelper {
        public static EmployeeRole GetUserRole(ClaimsPrincipal user) {
            var roleString = user.FindFirst("employeeRole")?.Value ?? null;
            if (roleString == null)
                throw new NoRoleClaimToCurrentUserException(message: "Cannot found claim with user role information");

            var role = EmployeeRoleExtension.StringToRole(roleString);
            if (role == null)
                throw new UnknownUserRoleException(message: $"{roleString} is unknown role");

            return (EmployeeRole)role;
        }

        public static int GetUserId(ClaimsPrincipal user) {
            var userIdString = user.FindFirstValue("employeeId");
            return Convert.ToInt32(userIdString);
        }
    }
}
