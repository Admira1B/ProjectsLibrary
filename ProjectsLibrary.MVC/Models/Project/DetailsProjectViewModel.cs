using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.DTOs.Project;

namespace ProjectsLibrary.MVC.Models.Project
{
    public class DetailsProjectViewModel
    {
        public int Id { get; set; }
        public ProjectUpdateDto Project { get; set; } = null!; 
        public List<EmployeeReadDto> Employees { get; set; } = [];
        public List<CompanyReadDto> Companies { get; set; } = [];
    }
}
