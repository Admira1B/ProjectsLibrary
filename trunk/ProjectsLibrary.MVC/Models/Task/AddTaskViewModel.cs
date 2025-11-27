using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.DTOs.Task;

namespace ProjectsLibrary.MVC.Models.Task
{
    public class AddTaskViewModel
    {
        public int? SelectedProjectId { get; set; }
        public TaskAddDto? Task { get; set; }
        public List<ProjectReadDto> Projects { get; set; } = [];
        public List<EmployeeReadDto> Employees { get; set; } = [];
    }
}
