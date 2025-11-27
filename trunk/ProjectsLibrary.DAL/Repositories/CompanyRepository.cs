using Microsoft.EntityFrameworkCore;
using ProjectsLibrary.Domain.Contracts.Repositories;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectsLibrary.DAL.Repositories
{
    public class CompanyRepository(ProjectsLibraryDbContext context) : ICompanyRepository
    {
        private readonly ProjectsLibraryDbContext _context = context;

        public async Task Add(Company company)
        {
            await _context.Companies.AddAsync(company);
        }

        public async Task Delete(int id)
        {
            await _context.Companies.Where(c => c.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Companies.AnyAsync(c => c.Id == id);
        }

        public IQueryable<Company> Get()
        {
            return _context.Companies.AsNoTracking();
        }

        public IQueryable<Company> GetDataOnly()
        {
            return _context.Companies.AsNoTracking();
        }

        public async Task<Company> GetById(int id)
        {
            return await _context.Companies.Where(c => c.Id == id).FirstAsync();
        }

        public async Task<Company> GetByIdNoTracking(int id)
        {
            return await _context.Companies.AsNoTracking().Where(c => c.Id == id).FirstAsync();
        }

        public async Task<Company> GetCompanyWithProjectsNoTracking(int id) 
        {
            return await _context.Companies
                .Include(c => c.Projects)
                    .ThenInclude(p => p.ProjectManager)
                .AsNoTracking().Where(c => c.Id == id).FirstAsync();
        }

        public async Task Update(Company company)
        {
            await _context.Companies.Where(c => c.Id == company.Id).ExecuteUpdateAsync(x => x
            .SetProperty(c => c.Name, company.Name));
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
