using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectsLibrary.DAL.Configurations {
    public class CompanyConfiguration : IEntityTypeConfiguration<Company> {
        public void Configure(EntityTypeBuilder<Company> builder) {
            builder.HasKey(c => c.Id);

            builder.HasMany(c => c.Projects)
                .WithOne(p => p.Company)
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
