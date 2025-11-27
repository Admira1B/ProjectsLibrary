using Microsoft.EntityFrameworkCore;
using ProjectsLibrary.Domain.Contracts.Repositories;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectsLibrary.DAL.Repositories {
    public class TaskRepository(ProjectsLibraryDbContext context) : ITaskRepository {
        private readonly ProjectsLibraryDbContext _context = context;

        public async Task Add(TaskPL task) {
            await _context.Tasks.AddAsync(task);
        }

        public async Task Delete(int id) {
            await _context.Tasks.Where(t => t.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> ExistsAsync(int id) {
            return await _context.Tasks.AnyAsync(t => t.Id == id);
        }

        public IQueryable<TaskPL> Get() {
            return _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Creator)
                .Include(t => t.Executor)
                .AsNoTracking();
        }
        public IQueryable<TaskPL> GetDataOnly() {
            return _context.Tasks.AsNoTracking();
        }

        public async Task<TaskPL> GetById(int id) {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Creator)
                .Include(t => t.Executor)
                .Where(t => t.Id == id).FirstAsync();
        }

        public async Task<TaskPL> GetByIdNoTracking(int id) {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Creator)
                .Include(t => t.Executor)
                .AsNoTracking().Where(t => t.Id == id).FirstAsync();
        }

        public async Task<int> SaveChangesAsync() {
            return await _context.SaveChangesAsync();
        }

        public async Task Update(TaskPL task) {
            await _context.Tasks.Where(t => t.Id == task.Id).ExecuteUpdateAsync(x => x
            .SetProperty(t => t.Name, task.Name)
            .SetProperty(t => t.ProjectId, task.ProjectId)
            .SetProperty(t => t.CreatorId, task.CreatorId)
            .SetProperty(t => t.ExecutorId, task.ExecutorId));
        }
    }
}
