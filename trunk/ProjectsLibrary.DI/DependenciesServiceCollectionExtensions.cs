using Microsoft.Extensions.DependencyInjection;
using ProjectsLibrary.DAL.Repositories;
using ProjectsLibrary.Domain.Contracts.Repositories;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Services;

namespace ProjectsLibrary.CompositionRoot
{
    public static class DependenciesServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccessDependencies(this IServiceCollection services) 
        {
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();

            return services;
        }

        public static IServiceCollection AddServicesDependencies(this IServiceCollection services) 
        {
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}
