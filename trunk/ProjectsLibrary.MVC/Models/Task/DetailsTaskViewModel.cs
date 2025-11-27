using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.DTOs.Employee;

namespace ProjectsLibrary.MVC.Models.Task {
    public class DetailsTaskViewModel {
        public int Id { get; set; }
        public TaskUpdateDto Task { get; set; } = null!;
        public List<ProjectReadDto> Projects { get; set; } = [];
        public List<EmployeeReadDto> Employees { get; set; } = [];
        public EmployeeShortDto TaskCreator { get; set; } = null!;
    }
}
