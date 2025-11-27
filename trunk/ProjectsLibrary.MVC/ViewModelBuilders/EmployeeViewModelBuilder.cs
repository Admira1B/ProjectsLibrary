using AutoMapper;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.MVC.Helpers;
using ProjectsLibrary.MVC.Models.Employee;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;
using System.Security.Claims;

namespace ProjectsLibrary.MVC.ViewModelBuilders {
    public class EmployeeViewModelBuilder(IEmployeeService service, IMapper mapper) : IEmployeeViewModelBuilder {
        private readonly IEmployeeService _service = service;
        private readonly IMapper _mapper = mapper;

        public Task<IndexEmployeeViewModel> BuildIndexViewModelAsync(ClaimsPrincipal user) {
            var model = new IndexEmployeeViewModel {
                UserRole = UserHelper.GetUserRole(user)
            };

            return Task.FromResult(model);
        }

        public async Task<AddEmployeeViewModel> BuildAddViewModelAsync(EmployeeAddDto? employeeDto = null) {
            var model = new AddEmployeeViewModel() {
                Employee = employeeDto
            };

            return model;
        }

        public async Task<DetailsEmployeeViewModel?> BuildDetailsViewModelAsync(int id, EmployeeUpdateDto? employeeDto = null) {
            var employee = await _service.GetByIdNoTrackingAsync(id);

            if (employee == null) {
                return null;
            }

            var model = new DetailsEmployeeViewModel() {
                Id = id,
                Employee = employeeDto ?? _mapper.Map<EmployeeUpdateDto>(employee)
            };

            return model;
        }
    }
}
