using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.DTOs.Task;

namespace ProjectsLibrary.DTOs.Project
{
    public class ProjectReadDto
    {
        public int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<EmployeeShortDto> Employees { get; set; } = [];
        public List<TaskShortDto> Tasks { get; set; } = [];
        public int ProjectManagerId { get; set; }
        public EmployeeShortDto? ProjectManager { get; set; }
        public int CompanyId { get; set; }
        public CompanyShortDto? Company { get; set; }
    }
}
