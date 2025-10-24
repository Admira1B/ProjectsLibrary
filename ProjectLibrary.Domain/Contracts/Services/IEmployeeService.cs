using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;

namespace ProjectsLibrary.Domain.Contracts.Services
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAsync();
        Task<List<Employee>> GetDataOnlyAsync();
        Task<PagedResult<Employee>> GetPaginatedAsync(FilterParams filterParams, SortParams sortParams, PageParams pageParams);
        Task<Employee> GetByIdAsync(int id);
        Task<Employee> GetByIdNoTrackingAsync(int id);
        Task<Employee> GetEmployeeWithTasksNoTrackingAsync(int id);
        Task<Employee> GetEmployeeWithProjectsNoTrackingAsync(int id);
        Task RegisterAsync(Employee employee, string password);
        Task<string> LoginAsync(string email, string password);
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(int id);
        Task AssignTaskToEmployee(int employeeId, int taskId);
        Task UnassignTaskToEmployee(int employeeId, int taskId);
    }
}
