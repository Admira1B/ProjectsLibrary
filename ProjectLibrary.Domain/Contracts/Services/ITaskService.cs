using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;

namespace ProjectsLibrary.Domain.Contracts.Services
{
    public interface ITaskService
    {
        Task<List<TaskPL>> GetAsync();
        Task<List<TaskPL>> GetDataOnlyAsync();
        Task<PagedResult<TaskPL>> GetPaginatedAsync(FilterParams filterParams, SortParams sortParams, PageParams pageParams);
        Task<TaskPL> GetByIdAsync(int id);
        Task<TaskPL> GetByIdNoTrackingAsync(int id);
        Task AddAsync(TaskPL task);
        Task UpdateAsync(TaskPL task);
        Task DeleteAsync(int id);
    }
}
