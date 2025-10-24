using ProjectsLibrary.Domain.Models.Enums;

namespace ProjectsLibrary.Domain.Models.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public EmployeeRole Role { get; set; } = EmployeeRole.Employee;
        public string PasswordHash { get; set; } = string.Empty;
        public List<TaskPL> ExecutingTasks { get; set; } = [];
        public List<TaskPL> CreatedTasks { get; set; } = [];
        public List<Project> WorkingProjects { get; set; } = [];
        public List<Project> ManagedProjects { get; set; } = [];
    }
}
