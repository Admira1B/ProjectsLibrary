using Microsoft.EntityFrameworkCore;
using ProjectsLibrary.Domain.Contracts.Repositories;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Exceptions;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.Services.Extencions;

namespace ProjectsLibrary.Services
{
    public class ProjectService(IProjectRepository repository, IEmployeeService employeeService) : IProjectService
    {
        private readonly IProjectRepository _repository = repository;
        private readonly IEmployeeService _employeeService = employeeService;

        public async Task AddAsync(Project project)
        {
            project.StartDate = DateTime.UtcNow;

            await _repository.Add(project);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await EnsureProjectExistsAsync(id);
            await _repository.Delete(id);
        }

        public async Task<List<Project>> GetAsync()
        {
            var query = _repository.Get();
            return await query.ToListAsync();
        }

        public async Task<List<Project>> GetDataOnlyAsync()
        {
            var query = _repository.GetDataOnly();
            return await query.ToListAsync();
        }

        public async Task<PagedResult<Project>> GetPaginatedAsync(FilterParams filterParams, SortParams sortParams, PageParams pageParams)
        {
            var allProjects = _repository.Get();
            var filteredProjects = allProjects.Filter(filterParams).Sort(sortParams);
            var paginatedProjects = await filteredProjects.Paginate(pageParams).ToListAsync();

            var result = new PagedResult<Project>()
            {
                Datas = paginatedProjects,
                FilteredRecords = filteredProjects.Count(),
                TotalRecords = allProjects.Count(),
            };

            return result;
        }

        public async Task<Project> GetByIdNoTrackingAsync(int id)
        {
            await EnsureProjectExistsAsync(id);
            return await _repository.GetByIdNoTracking(id);
        }

        public async Task<Project> GetByIdAsync(int id)
        {
            await EnsureProjectExistsAsync(id);
            return await _repository.GetById(id);
        }

        public async Task<Project> GetProjectWithTasksNoTrackingAsync(int id)
        {
            await EnsureProjectExistsAsync(id);
            return await _repository.GetProjectWithTasksNoTracking(id);
        }

        public async Task UpdateAsync(Project project)
        {
            await EnsureProjectExistsAsync(project.Id);

            // Project manager cannot work on the project
            var projectEmployees = (await GetByIdAsync(project.Id)).Employees;
            var employeeWorkingOnProject = projectEmployees.Where(e => e.Id == project.ProjectManagerId).FirstOrDefault();

            if (employeeWorkingOnProject != null)
            {
                projectEmployees.Remove(employeeWorkingOnProject);
                await _repository.SaveChangesAsync();
            }

            await _repository.Update(project);
        }

        public async Task AddEmployeeToProject(int projectId, int employeeId)
        {
            var project = await GetByIdAsync(projectId);
            var employee = await _employeeService.GetByIdAsync(employeeId);

            if (project.ProjectManagerId == employeeId)
            {
                throw new EntityCollectionModificationException(
                    message: "Employee is project manager",
                    details: $"Employee with id {employeeId} is project manager on this project");
            }

            if (project.Employees.Any(e => e.Id == employeeId))
            {
                throw new EntityCollectionModificationException(
                    message: "The employee is already working on this project",
                    details: $"Employee with id {employeeId} already exists on this project");
            }

            project.Employees.Add(employee);
            await _repository.SaveChangesAsync();
        }

        public async Task RemoveEmployeeFromProject(int projectId, int employeeId)
        {
            var project = await GetByIdAsync(projectId);
            var employee = await _employeeService.GetByIdAsync(employeeId);

            if (project.ProjectManagerId == employeeId)
            {
                throw new EntityCollectionModificationException(
                    message: "Employee is project manager",
                    details: $"Employee with id {employeeId} is project manager on this project");
            }

            if (!project.Employees.Any(e => e.Id == employeeId))
            {
                throw new EntityCollectionModificationException(
                    message: "The employee is not working on this project",
                    details: $"Employee with id {employeeId} doesn`t exists on this project");
            }

            project.Employees.Remove(employee);
            await _repository.SaveChangesAsync();
        }

        private async Task EnsureProjectExistsAsync(int id)
        {
            if (!await _repository.ExistsAsync(id))
            {
                throw new EntityNotFoundException(
                    message: "Project not found",
                    details: $"Project with id {id} doesn't exists");
            }
        }
    }
}
