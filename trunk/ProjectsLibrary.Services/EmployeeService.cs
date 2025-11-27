using Microsoft.EntityFrameworkCore;
using ProjectsLibrary.Domain.Contracts.Repositories;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Exceptions;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.Services.Extencions;

namespace ProjectsLibrary.Services {
    public class EmployeeService(IEmployeeRepository repository, ITaskService taskService, IPasswordHasherService passwordHasher, IJwtService jwtService) : IEmployeeService {
        private readonly IEmployeeRepository _repository = repository;
        private readonly ITaskService _taskService = taskService;
        private readonly IPasswordHasherService _passwordHasher = passwordHasher;
        private readonly IJwtService _jwtService = jwtService;

        public async Task RegisterAsync(Employee employee, string password) {
            var existingEmployee = await GetByEmailAsync(employee.Email);

            if (existingEmployee != null) {
                if (string.IsNullOrWhiteSpace(existingEmployee.PasswordHash)) {
                    existingEmployee.PasswordHash = _passwordHasher.GetPasswordHash(password);
                    await UpdateAsync(existingEmployee);
                    return;
                }

                throw new EmployeeAlreadyExistsException(
                    message: "Employee already exists",
                    details: $"Employee with email {employee.Email} exists");
            }

            employee.PasswordHash = _passwordHasher.GetPasswordHash(password);

            await _repository.Add(employee);
            await _repository.SaveChangesAsync();
        }

        public async Task<string> LoginAsync(string email, string password) {
            var employee = await GetByEmailAsync(email);

            if (employee == null) {
                throw new EmailNotRegisteredException(
                    message: "Invalid email",
                    details: $"Employee with email {email} doesn`t exists");
            }

            if (string.IsNullOrWhiteSpace(employee.PasswordHash)) {
                throw new CreatedEmployeeNotRegisteredException(
                    message: "Employee not registered",
                    details: $"Employee with email {email} exists, but not registered");
            }

            if (!_passwordHasher.VerifyPassword(password, employee.PasswordHash)) {
                throw new IncorrectEmployeePasswordException(
                    message: "Invalid password",
                    details: $"Invalid password for user with email {email}");
            }

            var token = _jwtService.GenerateWebToken(employee);

            return token;
        }

        public async Task AddAsync(Employee employee) {
            await _repository.Add(employee);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id) {
            await EnsureEmployeeExistsAsync(id);
            await _repository.Delete(id);
        }

        public async Task<List<Employee>> GetAsync() {
            var query = _repository.Get();
            return await query.ToListAsync();
        }

        public async Task<List<Employee>> GetDataOnlyAsync() {
            var query = _repository.GetDataOnly();
            return await query.ToListAsync();
        }

        public async Task<PagedResult<Employee>> GetPaginatedAsync(FilterParams filterParams, SortParams sortParams, PageParams pageParams) {
            var allEmployees = _repository.Get();
            var filteredEmployees = allEmployees.Filter(filterParams).Sort(sortParams);
            var paginatedEmployees = await filteredEmployees.Paginate(pageParams).ToListAsync();

            var result = new PagedResult<Employee>() {
                Datas = paginatedEmployees,
                FilteredRecords = filteredEmployees.Count(),
                TotalRecords = allEmployees.Count(),
            };

            return result;
        }

        public async Task<Employee> GetByIdNoTrackingAsync(int id) {
            await EnsureEmployeeExistsAsync(id);
            return await _repository.GetByIdNoTracking(id);
        }
        public async Task<Employee> GetByIdAsync(int id) {
            await EnsureEmployeeExistsAsync(id);
            return await _repository.GetById(id);
        }

        public async Task<Employee> GetEmployeeWithTasksNoTrackingAsync(int id) {
            await EnsureEmployeeExistsAsync(id);
            return await _repository.GetEmployeeWithTasksNoTracking(id);
        }

        public async Task<Employee> GetEmployeeWithProjectsNoTrackingAsync(int id) {
            await EnsureEmployeeExistsAsync(id);
            return await _repository.GetEmployeeWithProjectsNoTracking(id);
        }

        public async Task<List<Project>> GetEmployeeAllProjectsByIdNoTrackingAsync(int id) {
            var employee = await GetByIdAsync(id);
            var managedProjects = employee.ManagedProjects;
            var workingProjects = employee.WorkingProjects;

            var result = managedProjects.Concat(workingProjects).ToList();

            return result;
        }

        public async Task UpdateAsync(Employee employee) {
            await EnsureEmployeeExistsAsync(employee.Id);
            await _repository.Update(employee);
        }

        public async Task AssignTaskToEmployee(int employeeId, int taskId) {
            var employee = await GetByIdAsync(employeeId);
            var task = await _taskService.GetByIdAsync(taskId);

            if (employee.ExecutingTasks.Any(t => t.Id == taskId)) {
                throw new EntityCollectionModificationException(
                    message: "The employee is already working on this task",
                    details: $"Task with id {taskId} already exists into employee`s tasks");
            }

            employee.ExecutingTasks.Add(task);
            await _repository.SaveChangesAsync();
        }

        public async Task UnassignTaskToEmployee(int employeeId, int taskId) {
            var employee = await GetByIdAsync(employeeId);
            var task = await _taskService.GetByIdAsync(taskId);

            if (!employee.ExecutingTasks.Any(t => t.Id == taskId)) {
                throw new EntityCollectionModificationException(
                    message: "The employee is not working on this task",
                    details: $"Task with id {taskId} doesn`t exists into employee`s tasks");
            }

            employee.ExecutingTasks.Remove(task);
            await _repository.SaveChangesAsync();
        }

        private async Task EnsureEmployeeExistsAsync(int id) {
            if (!await _repository.ExistsAsync(id)) {
                throw new EntityNotFoundException(
                    message: "Employee not found",
                    details: $"Employee with id {id} doesn't exists");
            }
        }

        private async Task<Employee?> GetByEmailAsync(string email) {
            return await _repository.GetDataOnly()
                .Where(e => e.Email.ToLower() == email.ToLower())
                .FirstOrDefaultAsync();
        }
    }
}
