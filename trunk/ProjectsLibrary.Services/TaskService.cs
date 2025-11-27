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
    public class TaskService(ITaskRepository repository) : ITaskService
    {
        private readonly ITaskRepository _repository = repository;

        public async Task AddAsync(TaskPL task)
        {
            await _repository.Add(task);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await EnsureTaskExistsAsync(id);
            await _repository.Delete(id);
        }

        public async Task<List<TaskPL>> GetAsync()
        {
            var query = _repository.Get();
            return await query.ToListAsync();
        }

        public async Task<List<TaskPL>> GetDataOnlyAsync()
        {
            var query = _repository.GetDataOnly();
            return await query.ToListAsync();
        }

        public async Task<PagedResult<TaskPL>> GetPaginatedAsync(FilterParams filterParams, SortParams sortParams, PageParams pageParams, int? employeeId = null)
        {
            IQueryable<TaskPL> tasks;


            if (employeeId != null)
            {
                tasks = _repository.Get().Where(t => t.CreatorId == employeeId || t.ExecutorId == employeeId || t.Project.ProjectManagerId == employeeId);
            }
            else
            {
                tasks = _repository.Get();
            }

            var filteredTasks = tasks.Filter(filterParams).Sort(sortParams);
            var paginatedTasks = await filteredTasks.Paginate(pageParams).ToListAsync();

            var result = new PagedResult<TaskPL>()
            {
                Datas = paginatedTasks,
                FilteredRecords = filteredTasks.Count(),
                TotalRecords = tasks.Count(),
            };

            return result;
        }

        public async Task<TaskPL> GetByIdNoTrackingAsync(int id)
        {
            await EnsureTaskExistsAsync(id);
            return await _repository.GetByIdNoTracking(id);
        }

        public async Task<TaskPL> GetByIdAsync(int id)
        {
            await EnsureTaskExistsAsync(id);
            return await _repository.GetById(id);
        }

        public async Task UpdateAsync(TaskPL task)
        {
            await EnsureTaskExistsAsync(task.Id);
            await _repository.Update(task);
        }

        private async Task EnsureTaskExistsAsync(int id)
        {
            if (!await _repository.ExistsAsync(id))
            {
                throw new EntityNotFoundException(
                    message: "Task not found",
                    details: $"Task with id {id} doesn't exists");
            }
        }
    }
}
