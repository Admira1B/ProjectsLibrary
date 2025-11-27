using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.MVC.Models.Employee;
using System.Security.Claims;

namespace ProjectsLibrary.MVC.ViewModelBuilders.Interfaces {
    public interface IEmployeeViewModelBuilder {
        Task<AddEmployeeViewModel> BuildAddViewModelAsync(EmployeeAddDto? employeeDto = null);
        Task<DetailsEmployeeViewModel?> BuildDetailsViewModelAsync(int id, EmployeeUpdateDto? employeeDto = null);
        Task<IndexEmployeeViewModel> BuildIndexViewModelAsync(ClaimsPrincipal user);
    }
}