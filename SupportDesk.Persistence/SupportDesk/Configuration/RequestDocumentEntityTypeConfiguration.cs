using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Persistence.SupportDesk.Configuration
{
    public class RequestDocumentEntityTypeConfiguration : IEntityTypeConfiguration<RequestDocument>
    {
        public void Configure(EntityTypeBuilder<RequestDocument> builder)
        {
            builder.ToTable("RequestDocuments");

            builder.HasKey(rd => rd.Id);

            builder.Property(rd => rd.Id).IsRequired();
            builder.Property(rd => rd.DocumentUrl).HasMaxLength(800).IsRequired();
            builder.Property(rd => rd.IsActive).IsRequired();

            builder.HasOne(rd => rd.Request)
                .WithMany(r => r.RequestDocuments)
                .HasForeignKey(rd => rd.RequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
