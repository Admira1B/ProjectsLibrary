using ProjectsLibrary.DTOs.Employee;

namespace ProjectsLibrary.MVC.Models.Employee
{
    public class DetailsEmployeeViewModel
    {
        public int Id { get; set; }
        public EmployeeUpdateDto Employee { get; set; } = null!;
    }
}
