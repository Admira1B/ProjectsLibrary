using ProjectsLibrary.Domain.Models.Enums;

namespace ProjectsLibrary.Domain.Models.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public EmployeeRole Role { get; set; } = EmployeeRole.Employee;
        public List<TaskPL> ExecutingTasks { get; set; } = [];
        public List<TaskPL> CreatedTasks { get; set; } = [];
        public List<Project> WorkingProjects { get; set; } = [];
        public List<Project> ManagedProjects { get; set; } = [];
    }
}
