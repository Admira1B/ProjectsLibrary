using Microsoft.EntityFrameworkCore;
using ProjectsLibrary.Domain.Contracts.Repositories;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Exceptions;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.Services.Extencions;

namespace ProjectsLibrary.Services {
    public class CompanyService(ICompanyRepository repository) : ICompanyService {
        private readonly ICompanyRepository _repository = repository;

        public async Task AddAsync(Company company) {
            await _repository.Add(company);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id) {
            await EnsureCompanyExistsAsync(id);
            await _repository.Delete(id);
        }

        public async Task<List<Company>> GetAsync() {
            var query = _repository.Get();
            return await query.ToListAsync();
        }
        public async Task<List<Company>> GetDataOnlyAsync() {
            var query = _repository.GetDataOnly();
            return await query.ToListAsync();
        }

        public async Task<PagedResult<Company>> GetPaginatedAsync(FilterParams filterParams, SortParams sortParams, PageParams pageParams) {
            var allCompanies = _repository.Get();
            var filteredCompanies = allCompanies.Filter(filterParams).Sort(sortParams);
            var paginatedCompanies = await filteredCompanies.Paginate(pageParams).ToListAsync();

            var result = new PagedResult<Company>() {
                Datas = paginatedCompanies,
                FilteredRecords = filteredCompanies.Count(),
                TotalRecords = allCompanies.Count(),
            };

            return result;
        }

        public async Task<Company> GetByIdNoTrackingAsync(int id) {
            await EnsureCompanyExistsAsync(id);
            return await _repository.GetByIdNoTracking(id);
        }

        public async Task<Company> GetByIdAsync(int id) {
            await EnsureCompanyExistsAsync(id);
            return await _repository.GetById(id);
        }

        public async Task<Company> GetCompanyWithProjectsNoTrackingAsync(int id) {
            await EnsureCompanyExistsAsync(id);
            return await _repository.GetCompanyWithProjectsNoTracking(id);
        }

        public async Task UpdateAsync(Company company) {
            await EnsureCompanyExistsAsync(company.Id);
            await _repository.Update(company);
        }

        private async Task EnsureCompanyExistsAsync(int id) {
            if (!await _repository.ExistsAsync(id)) {
                throw new EntityNotFoundException(
                    message: "Company not found",
                    details: $"Company with id {id} doesn't exists");
            }
        }
    }
}
