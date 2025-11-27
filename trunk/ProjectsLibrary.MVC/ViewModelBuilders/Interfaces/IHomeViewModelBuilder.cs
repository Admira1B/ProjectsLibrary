using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.MVC.Models.Home;

namespace ProjectsLibrary.MVC.ViewModelBuilders.Interfaces {
    public interface IHomeViewModelBuilder {
        Task<LoginViewModel> BuildLoginViewModelAsync(EmployeeLoginDto? employeeDto = null);
        Task<RegistrationViewModel> BuildRegistrationViewModelAsync(string email, EmployeeAddDto? employeeDto = null);
    }
}