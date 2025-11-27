using Microsoft.AspNetCore.Authorization;
using ProjectsLibrary.Domain.Models.Enums;

namespace ProjectsLibrary.CompositionRoot.Autorization {
    internal class RoleAuthorizationHandler : AuthorizationHandler<RoleAuthorizationRequirement> {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RoleAuthorizationRequirement requirement) {
            if (context.User.HasClaim(c => c.Type == "employeeRole")) {
                var roleClaim = context.User.FindFirst(c => c.Type == "employeeRole")?.Value;

                var role = EmployeeRoleExtension.StringToRole(roleClaim);

                if (role != null) {
                    if (role >= requirement.Role) {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
