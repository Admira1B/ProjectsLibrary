using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectsLibrary.DAL.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasMany(e => e.CreatedTasks)
                .WithOne(t => t.Creator)
                .HasForeignKey(e => e.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.ExecutingTasks)
                .WithOne(t => t.Executor)
                .HasForeignKey(t => t.ExecutorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.ManagedProjects)
                .WithOne(p => p.ProjectManager)
                .HasForeignKey(p => p.ProjectManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.WorkingProjects)
                .WithMany(p => p.Employees);
        }
    }
}
