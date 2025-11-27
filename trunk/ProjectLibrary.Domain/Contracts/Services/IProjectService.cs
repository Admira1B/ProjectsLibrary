using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;

namespace ProjectsLibrary.Domain.Contracts.Services {
    public interface IProjectService {
        Task<List<Project>> GetAsync();
        Task<List<Project>> GetDataOnlyAsync();
        Task<PagedResult<Project>> GetPaginatedAsync(FilterParams filterParams, SortParams sortParams, PageParams pageParams, int? employeeId = null);
        Task<Project> GetByIdAsync(int id);
        Task<Project> GetByIdNoTrackingAsync(int id);
        Task<Project> GetProjectWithTasksNoTrackingAsync(int id);
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(int id);
        Task AddEmployeeToProject(int projectId, int employeeId);
        Task RemoveEmployeeFromProject(int projectId, int employeeId);
    }
}

