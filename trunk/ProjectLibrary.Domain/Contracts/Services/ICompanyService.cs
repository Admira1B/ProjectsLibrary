using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;

namespace ProjectsLibrary.Domain.Contracts.Services
{
    public interface ICompanyService
    {
        Task<List<Company>> GetAsync();
        Task<List<Company>> GetDataOnlyAsync();
        Task<PagedResult<Company>> GetPaginatedAsync(FilterParams filterParams, SortParams sortParams, PageParams pageParams);
        Task<Company> GetByIdAsync(int id);
        Task<Company> GetByIdNoTrackingAsync(int id);
        Task<Company> GetCompanyWithProjectsNoTrackingAsync(int id);
        Task AddAsync(Company company);
        Task UpdateAsync(Company company);
        Task DeleteAsync(int id);
    }
}