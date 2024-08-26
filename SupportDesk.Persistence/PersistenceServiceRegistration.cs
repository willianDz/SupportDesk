using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Persistence.SupportDesk;
using SupportDesk.Persistence.SupportDesk.Repositories;

namespace SupportDesk.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IAsyncRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Aplicar las migraciones automáticamente
            try
            {
                var context = services.BuildServiceProvider().GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();  // Aplica todas las migraciones pendientes
            }
            catch (Exception ex)
            {
                // Manejar cualquier error en la migración
                Console.WriteLine($"Ocurrió un error al aplicar las migraciones: {ex.Message}");
            }

            return services;
        }
    }
}
