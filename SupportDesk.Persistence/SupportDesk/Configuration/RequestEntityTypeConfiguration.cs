using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Persistence.SupportDesk.Configuration
{
    public class RequestEntityTypeConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.ToTable("Requests");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).IsRequired();
            builder.Property(r => r.Comments).HasMaxLength(800).IsRequired();
            builder.Property(r => r.StartReviewDate).IsRequired(false);
            builder.Property(r => r.ApprovalRejectionDate).IsRequired(false);

            builder.HasOne(r => r.RequestStatus)
                .WithMany(rs => rs.Requests)
                .HasForeignKey(r => r.RequestStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.RequestType)
                .WithMany(rt => rt.Requests)
                .HasForeignKey(r => r.RequestTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Zone)
                .WithMany(z => z.Requests)
                .HasForeignKey(r => r.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ReviewerUser)
                .WithMany(u => u.ReviewedRequests)
                .HasForeignKey(r => r.ReviewerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.RequestDocuments)
                .WithOne(rd => rd.Request)
                .HasForeignKey(rd => rd.RequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
