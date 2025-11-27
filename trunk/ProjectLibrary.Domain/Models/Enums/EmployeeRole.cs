using System.Reflection.Emit;

namespace ProjectsLibrary.Domain.Models.Enums {
    public enum EmployeeRole {
        Employee = 0,
        Manager = 1,
        Supervisor = 2,
        Admin = 3,
    }

    public static class EmployeeRoleExtension {
        public static string RoleToString(this EmployeeRole role) {
            switch (role) {
                default:
                    return role.ToString();
            }
        }

        public static EmployeeRole? StringToRole(string? roleString) {
            return Enum.TryParse<EmployeeRole>(roleString, true, out var role)
                ? role : null;
        }
    }
}
