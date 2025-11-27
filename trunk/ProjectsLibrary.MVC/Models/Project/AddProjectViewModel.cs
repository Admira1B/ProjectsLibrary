using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.DTOs.Project;

namespace ProjectsLibrary.MVC.Models.Project {
    public class AddProjectViewModel {
        public ProjectAddDto? Project { get; set; }
        public List<EmployeeReadDto> Employees { get; set; } = [];
        public List<CompanyReadDto> Companies { get; set; } = [];
    }
}
