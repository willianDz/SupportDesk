using Microsoft.EntityFrameworkCore;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Persistence.SupportDesk
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<RequestType> RequestTypes { get; set; }
        public DbSet<UserZone> UserZones { get; set; }
        public DbSet<UserRequestType> UserRequestTypes { get; set; }
        public DbSet<RequestStatus> RequestStatuses { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestDocument> RequestDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }

}
