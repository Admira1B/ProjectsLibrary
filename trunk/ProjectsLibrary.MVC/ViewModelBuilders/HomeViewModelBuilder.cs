using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.MVC.Models.Home;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;

namespace ProjectsLibrary.MVC.ViewModelBuilders {
    public class HomeViewModelBuilder : IHomeViewModelBuilder {
        public Task<LoginViewModel> BuildLoginViewModelAsync(EmployeeLoginDto? employeeDto = null) {
            var model = new LoginViewModel() {
                Employee = employeeDto
            };

            return Task.FromResult(model);
        }

        public Task<RegistrationViewModel> BuildRegistrationViewModelAsync(string email, EmployeeAddDto? employeeDto = null) {
            var model = new RegistrationViewModel() {
                InitialEmail = email,
                Employee = employeeDto
            };

            return Task.FromResult(model);
        }
    }
}
