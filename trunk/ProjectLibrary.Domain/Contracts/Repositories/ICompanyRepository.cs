using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectsLibrary.Domain.Contracts.Repositories {
    public interface ICompanyRepository {
        Task Add(Company company);
        Task Delete(int id);
        Task<bool> ExistsAsync(int id);
        Task Update(Company company);
        IQueryable<Company> Get();
        IQueryable<Company> GetDataOnly();
        Task<Company> GetById(int id);
        Task<Company> GetByIdNoTracking(int id);
        Task<Company> GetCompanyWithProjectsNoTracking(int id);
        Task<int> SaveChangesAsync();
    }
}
