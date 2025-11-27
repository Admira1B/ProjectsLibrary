using Microsoft.AspNetCore.Authorization;
using ProjectsLibrary.Domain.Models.Enums;

namespace ProjectsLibrary.CompositionRoot.Autorization {
    internal class RoleAuthorizationRequirement(EmployeeRole role) : IAuthorizationRequirement {
        public EmployeeRole Role { get; } = role;
    }
}
