using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Persistence.SupportDesk.Configuration
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .IsRequired();

            builder.Property(u => u.Email)
                .IsRequired();

            builder.Property(u => u.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.BirthDate)
                .IsRequired(false);

            builder.Property(u => u.GenderId)
                .IsRequired(false);

            builder.Property(u => u.PhotoUrl)
                .IsRequired(false);

            builder.Property(u => u.IsAdmin)
                .IsRequired();

            builder.Property(u => u.IsSupervisor)
                .IsRequired();

            builder.HasOne(u => u.Gender)
                .WithMany(g => g.Users)
                .HasForeignKey(u => u.GenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.UserZones)
                .WithOne(uz => uz.User)
                .HasForeignKey(uz => uz.UserId);

            builder.HasMany(u => u.UserRequestTypes)
                .WithOne(urt => urt.User)
                .HasForeignKey(urt => urt.UserId);

            builder.HasMany(u => u.TwoFactorAuthTokens)
                .WithOne(tfa => tfa.User)
                .HasForeignKey(tfa => tfa.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.ReviewedRequests)
                .WithOne(r => r.ReviewerUser)
                .HasForeignKey(r => r.ReviewerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(new User
            {
                Id = Guid.NewGuid(),
                Email = "williandiaz0012@gmail.com",
                FirstName = "Administrador",
                LastName = "Administrador",
                IsAdmin = true,
                IsSupervisor = true
            });
        }
    }
}
