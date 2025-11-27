using Microsoft.EntityFrameworkCore;
using ProjectsLibrary.Domain.Contracts.Repositories;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectsLibrary.DAL.Repositories
{
    public class EmployeeRepository(ProjectsLibraryDbContext context) : IEmployeeRepository
    {
        private readonly ProjectsLibraryDbContext _context = context;

        public async Task Add(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public async Task Delete(int id)
        {
            await _context.Employees.Where(e => e.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(e => e.Id == id);
        }

        public IQueryable<Employee> Get()
        {
            return _context.Employees
                .Include(e => e.WorkingProjects)
                .Include(e => e.ExecutingTasks)
                .Include(e => e.CreatedTasks)
                .Include(e => e.ManagedProjects)
                .AsNoTracking();
        }

        public IQueryable<Employee> GetDataOnly()
        {
            return _context.Employees.AsNoTracking();
        }

        public async Task<Employee> GetById(int id)
        {
            return await _context.Employees
                .Include(e => e.WorkingProjects)
                .Include(e => e.ExecutingTasks)
                .Include(e => e.CreatedTasks)
                .Include(e => e.ManagedProjects)
                .Where(e => e.Id == id).FirstAsync();
        }

        public async Task<Employee> GetByIdNoTracking(int id)
        {
            return await _context.Employees
                .Include(e => e.WorkingProjects)
                .Include(e => e.ExecutingTasks)
                .Include(e => e.CreatedTasks)
                .Include(e => e.ManagedProjects)
                .AsNoTracking().Where(e => e.Id == id).FirstAsync();
        }

        public async Task<Employee> GetEmployeeWithTasksNoTracking(int id) 
        {
            return await _context.Employees
                .Include(e => e.ExecutingTasks)
                .Include(e => e.CreatedTasks)
                .AsNoTracking().Where(e => e.Id == id).FirstAsync();
        }

        public async Task<Employee> GetEmployeeWithProjectsNoTracking(int id) 
        {
            return await _context.Employees
                .Include(e => e.ManagedProjects)
                .Include(e => e.WorkingProjects)
                .AsNoTracking().Where(e => e.Id == id).FirstAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task Update(Employee employee)
        {
            await _context.Employees.Where(e => e.Id == employee.Id).ExecuteUpdateAsync(x => x
            .SetProperty(e => e.FirstName, employee.FirstName)
            .SetProperty(e => e.LastName, employee.LastName)
            .SetProperty(e => e.Email, employee.Email)
            .SetProperty(e => e.Role, employee.Role)
            .SetProperty(e => e.PasswordHash, employee.PasswordHash));
        }
    }
}
