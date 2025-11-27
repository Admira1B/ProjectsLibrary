using Microsoft.EntityFrameworkCore;
using ProjectsLibrary.Domain.Contracts.Repositories;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectsLibrary.DAL.Repositories {
    public class ProjectRepository(ProjectsLibraryDbContext context) : IProjectRepository {
        private readonly ProjectsLibraryDbContext _context = context;

        public async Task Add(Project project) {
            await _context.AddAsync(project);
        }

        public async Task Delete(int id) {
            await _context.Projects.Where(p => p.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> ExistsAsync(int id) {
            return await _context.Projects.Where(p => p.Id == id).AnyAsync();
        }

        public IQueryable<Project> Get() {
            return _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Company)
                .Include(p => p.Employees)
                .Include(p => p.ProjectManager)
                .AsNoTracking();
        }

        public IQueryable<Project> GetDataOnly() {
            return _context.Projects.AsNoTracking();
        }

        public async Task<Project> GetById(int id) {
            return await _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Company)
                .Include(p => p.Employees)
                .Include(p => p.ProjectManager)
                .Where(p => p.Id == id).FirstAsync();
        }

        public async Task<Project> GetByIdNoTracking(int id) {
            return await _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Company)
                .Include(p => p.Employees)
                .Include(p => p.ProjectManager)
                .AsNoTracking().Where(p => p.Id == id).FirstAsync();
        }

        public async Task<Project> GetProjectWithTasksNoTracking(int id) {
            return await _context.Projects
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.Creator)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.Executor)
                .AsNoTracking().Where(p => p.Id == id).FirstAsync();
        }

        public async Task<int> SaveChangesAsync() {
            return await _context.SaveChangesAsync();
        }

        public async Task Update(Project project) {
            await _context.Projects.Where(p => p.Id == project.Id).ExecuteUpdateAsync(x => x
            .SetProperty(p => p.Name, project.Name)
            .SetProperty(p => p.Priority, project.Priority)
            .SetProperty(p => p.StartDate, project.StartDate)
            .SetProperty(p => p.EndDate, project.EndDate)
            .SetProperty(p => p.ProjectManagerId, project.ProjectManagerId)
            .SetProperty(p => p.CompanyId, project.CompanyId));

        }
    }
}
