using ProjectsLibrary.Domain.Models.Enums;
using System.Security.Claims;

namespace ProjectLibrary.Tests {
    internal static class MockClaimHelper {
        public static ClaimsPrincipal BuildEmployeeClaim() {
            var user = new ClaimsPrincipal (
                new ClaimsIdentity ([
                        new Claim("employeeRole", EmployeeRole.Employee.RoleToString()),
                        new Claim("employeeId", 1.ToString()),
                    ], "mock" 
                )
            );

            return user;
        }

        public static ClaimsPrincipal BuildManagerClaim() {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity([
                        new Claim("employeeRole", EmployeeRole.Manager.RoleToString()),
                        new Claim("employeeId", 1.ToString()),
                    ], "mock"
                )
            );

            return user;
        }

        public static ClaimsPrincipal BuildSupervisorClaim() {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity([
                        new Claim("employeeRole", EmployeeRole.Supervisor.RoleToString()),
                        new Claim("employeeId", 1.ToString()),
                    ], "mock"
                )
            );

            return user;
        }

        public static ClaimsPrincipal BuildAdminClaim() {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity([
                        new Claim("employeeRole", EmployeeRole.Admin.RoleToString()),
                        new Claim("employeeId", 1.ToString()),
                    ], "mock"
                )
            );

            return user;
        }
    }
}
