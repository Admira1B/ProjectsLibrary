using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectsLibrary.Domain.Contracts.Repositories {
    public interface IProjectRepository {
        Task Add(Project project);
        Task Delete(int id);
        Task<bool> ExistsAsync(int id);
        Task Update(Project project);
        IQueryable<Project> Get();
        IQueryable<Project> GetDataOnly();
        Task<Project> GetById(int id);
        Task<Project> GetByIdNoTracking(int id);
        Task<Project> GetProjectWithTasksNoTracking(int id);
        Task<int> SaveChangesAsync();
    }
}
