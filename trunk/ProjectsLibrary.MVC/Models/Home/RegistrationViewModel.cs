using ProjectsLibrary.DTOs.Employee;

namespace ProjectsLibrary.MVC.Models.Home {
    public class RegistrationViewModel {
        public string? InitialEmail { get; set; } = string.Empty;
        public EmployeeAddDto? Employee { get; set; }
    }
}
