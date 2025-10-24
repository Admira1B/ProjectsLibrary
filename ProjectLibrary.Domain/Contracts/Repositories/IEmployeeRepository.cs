using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectsLibrary.Domain.Contracts.Repositories
{
    public interface IEmployeeRepository
    {
        Task Add(Employee employee);
        Task Delete(int id);
        Task<bool> ExistsAsync(int id);
        Task Update(Employee employee);
        IQueryable<Employee> Get();
        IQueryable<Employee> GetDataOnly();
        Task<Employee> GetById(int id);
        Task<Employee> GetByIdNoTracking(int id);
        Task<Employee> GetEmployeeWithTasksNoTracking(int id);
        Task<Employee> GetEmployeeWithProjectsNoTracking(int id);
        Task<int> SaveChangesAsync();
    }
}
