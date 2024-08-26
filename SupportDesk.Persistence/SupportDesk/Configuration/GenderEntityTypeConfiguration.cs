using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Persistence.SupportDesk.Configuration
{
    public class GenderEntityTypeConfiguration : IEntityTypeConfiguration<Gender>
    {
        public void Configure(EntityTypeBuilder<Gender> builder)
        {
            builder.ToTable("Genders");

            builder.HasKey(g => g.GenderId);

            builder.Property(g => g.GenderId)
                .IsRequired();

            builder.Property(g => g.Description)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(g => g.Abbreviation)
                .HasMaxLength(5)
                .IsRequired();

            builder.HasMany(g => g.Users)
                .WithOne(u => u.Gender)
                .HasForeignKey(u => u.GenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Gender { GenderId = 1, Description = "Masculino", Abbreviation = "M" },
                new Gender { GenderId = 2, Description = "Femenino", Abbreviation = "F" }
            );
        }
    }
}
