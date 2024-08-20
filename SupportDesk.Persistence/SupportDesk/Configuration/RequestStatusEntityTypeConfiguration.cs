using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Persistence.SupportDesk.Configuration
{
    public class RequestStatusEntityTypeConfiguration : IEntityTypeConfiguration<RequestStatus>
    {
        public void Configure(EntityTypeBuilder<RequestStatus> builder)
        {
            builder.ToTable("RequestStatuses");

            builder.HasKey(rs => rs.RequestStatusId);

            builder.Property(rs => rs.RequestStatusId).IsRequired();
            builder.Property(rs => rs.Description).HasMaxLength(100).IsRequired();
            builder.Property(rs => rs.Abbreviation).HasMaxLength(10).IsRequired();

            builder.HasMany(rs => rs.Requests)
                .WithOne(r => r.RequestStatus)
                .HasForeignKey(r => r.RequestStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new RequestStatus { RequestStatusId = (int)RequestStatusesEnum.New, Description = "Nuevo", Abbreviation = "NUE" },
                new RequestStatus { RequestStatusId = (int)RequestStatusesEnum.UnderReview, Description = "En Revision", Abbreviation = "ENR" },
                new RequestStatus { RequestStatusId = (int)RequestStatusesEnum.Rejected, Description = "Rechazado", Abbreviation = "REC" },
                new RequestStatus { RequestStatusId = (int)RequestStatusesEnum.Approved, Description = "Aprobado", Abbreviation = "APR" }
            );
        }
    }
}
