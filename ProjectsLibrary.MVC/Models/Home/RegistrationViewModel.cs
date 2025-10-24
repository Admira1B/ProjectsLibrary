using ProjectsLibrary.DTOs.Employee;

namespace ProjectsLibrary.MVC.Models.Home
{
    public class RegistrationViewModel
    {
        public string? InitialEmail { get; set; }
        public EmployeeAddDto? Employee { get; set; }
    }
}
