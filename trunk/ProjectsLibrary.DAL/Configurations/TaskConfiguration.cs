using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectsLibrary.DAL.Configurations {
    public class TaskConfiguration : IEntityTypeConfiguration<TaskPL> {
        public void Configure(EntityTypeBuilder<TaskPL> builder) {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Creator)
                .WithMany(e => e.CreatedTasks)
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Executor)
                .WithMany(e => e.ExecutingTasks)
                .HasForeignKey(t => t.ExecutorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
