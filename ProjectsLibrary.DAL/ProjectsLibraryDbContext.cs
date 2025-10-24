using Microsoft.EntityFrameworkCore;
using ProjectsLibrary.DAL.Configurations;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectsLibrary.DAL
{
    public class ProjectsLibraryDbContext : DbContext
    {
        public ProjectsLibraryDbContext(DbContextOptions<ProjectsLibraryDbContext> options) : base(options) { }

        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<TaskPL> Tasks { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new TaskConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
