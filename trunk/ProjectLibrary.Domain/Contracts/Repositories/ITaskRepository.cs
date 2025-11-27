using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectsLibrary.Domain.Contracts.Repositories
{
    public interface ITaskRepository
    {
        Task Add(TaskPL task);
        Task Delete(int id);
        Task<bool> ExistsAsync(int id);
        Task Update(TaskPL task);
        IQueryable<TaskPL> Get();
        IQueryable<TaskPL> GetDataOnly();
        Task<TaskPL> GetById(int id);
        Task<TaskPL> GetByIdNoTracking(int id);
        Task<int> SaveChangesAsync();
    }
}
