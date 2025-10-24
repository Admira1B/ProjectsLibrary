using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.DTOs.Task;

namespace ProjectsLibrary.DTOs.Employee
{
    public class EmployeeReadDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public List<TaskShortDto> ExecutingTasks { get; set; } = [];
        public List<TaskShortDto> CreatedTasks { get; set; } = [];
        public List<ProjectShortDto> WorkingProjects { get; set; } = [];
        public List<ProjectShortDto> ManagedProjects { get; set; } = [];
    }
}
