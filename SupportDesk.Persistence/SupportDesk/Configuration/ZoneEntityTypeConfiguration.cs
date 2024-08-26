using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Persistence.SupportDesk.Configuration
{
    public class ZoneEntityTypeConfiguration : IEntityTypeConfiguration<Zone>
    {
        public void Configure(EntityTypeBuilder<Zone> builder)
        {
            builder.ToTable("Zones");

            builder.HasKey(z => z.ZoneId);

            builder.Property(z => z.ZoneId)
                .IsRequired();

            builder.Property(z => z.Description)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(z => z.Abbreviation)
                .HasMaxLength(10)
                .IsRequired();

            builder.HasMany(z => z.UserZones)
                .WithOne(uz => uz.Zone)
                .HasForeignKey(uz => uz.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(z => z.Requests)
                .WithOne(r => r.Zone)
                .HasForeignKey(r => r.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Zone { ZoneId = 1, Description = "Zona Norte", Abbreviation = "ZN" },
                new Zone { ZoneId = 2, Description = "Zona Sur", Abbreviation = "ZS" },
                new Zone { ZoneId = 3, Description = "Zona Centro", Abbreviation = "ZC" },
                new Zone { ZoneId = 4, Description = "Zona Occidente", Abbreviation = "ZO" }
            );
        }
    }
}
