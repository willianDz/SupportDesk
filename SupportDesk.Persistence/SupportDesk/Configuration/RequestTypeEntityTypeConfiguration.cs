using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Persistence.SupportDesk.Configuration
{
    public class RequestTypeEntityTypeConfiguration : IEntityTypeConfiguration<RequestType>
    {
        public void Configure(EntityTypeBuilder<RequestType> builder)
        {
            builder.ToTable("RequestTypes");

            builder.HasKey(rt => rt.RequestTypeId);

            builder.Property(rt => rt.RequestTypeId)
                .IsRequired();

            builder.Property(rt => rt.Description)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(rt => rt.Abbreviation)
                .HasMaxLength(10)
                .IsRequired();

            builder.HasMany(rt => rt.UserRequestTypes)
                .WithOne(urt => urt.RequestType)
                .HasForeignKey(urt => urt.RequestTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(rt => rt.Requests)
                .WithOne(r => r.RequestType)
                .HasForeignKey(r => r.RequestTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new RequestType { RequestTypeId = 1, Description = "Tecnología de la información", Abbreviation = "TI" },
                new RequestType { RequestTypeId = 2, Description = "Recursos Humanos", Abbreviation = "RRHH" },
                new RequestType { RequestTypeId = 3, Description = "Legal", Abbreviation = "LEGA" },
                new RequestType { RequestTypeId = 4, Description = "Mantenimiento", Abbreviation = "MANT" },
                new RequestType { RequestTypeId = 5, Description = "Mercadeo", Abbreviation = "MKT" }
            );
        }
    }
}
