using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Persistence.SupportDesk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using SupportDesk.Application.Contracts.Infraestructure.Security;
using SupportDesk.Domain.Entities;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SupportDesk.Api.IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public string TestJwtToken { get; private set; } = string.Empty;

        public string TestSupervisorJwtToken { get; private set; } = string.Empty;

        public string TestAdminJwtToken { get; private set; } = string.Empty;

        private readonly User _testUser = new()
        {
            Id = Guid.NewGuid(),
            Email = "testuser@test.com",
            FirstName = "Test",
            LastName = "User",
            CreatedDate = DateTime.UtcNow,
            IsSupervisor = false,
            IsAdmin = false
        };

        private readonly User _testSupervisorUser = new()
        {
            Id = Guid.NewGuid(),
            Email = "testsupervisoruser@test.com",
            FirstName = "User",
            LastName = "Admin",
            CreatedDate = DateTime.UtcNow,
            IsSupervisor = true,
            IsAdmin = false
        };

        private readonly User _testAdminUser = new()
        {
            Id = Guid.NewGuid(),
            Email = "testadminuser@test.com",
            FirstName = "User",
            LastName = "Admin",
            CreatedDate = DateTime.UtcNow,
            IsSupervisor = false,
            IsAdmin = true
        };

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Eliminar la configuración de la base de datos existente.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Añadir una base de datos en memoria para las pruebas.
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Construir el ServiceProvider.
                var sp = services.BuildServiceProvider();

                // Crear el alcance y la base de datos de prueba.
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                // Asegurarse de que la base de datos esté creada.
                db.Database.EnsureCreated();

                // Sembrar datos de prueba, incluyendo el usuario de prueba.
                SeedTestData(db);

                // Generar un JWT para los usuarios de prueba.
                var tokenGenerator = scopedServices.GetRequiredService<ITokenGenerator>();

                TestJwtToken = GenerateJwtToken(_testUser, tokenGenerator);
                TestSupervisorJwtToken = GenerateJwtToken(_testSupervisorUser, tokenGenerator);
                TestAdminJwtToken = GenerateJwtToken(_testAdminUser, tokenGenerator);

                // Crear la carpeta FileStorage si no existe
                var storagePath = Path.Combine(Directory.GetCurrentDirectory(), "FileStorage");
                if (!Directory.Exists(storagePath))
                {
                    Directory.CreateDirectory(storagePath);
                }

                // Desactivar CSRF en el contexto de las pruebas
                services.AddAntiforgery(options => options.Cookie.Name = "Test-Antiforgery-Cookie");
                services.Configure<Microsoft.AspNetCore.Antiforgery.AntiforgeryOptions>(options =>
                {
                    options.SuppressXFrameOptionsHeader = true;
                });
            });


            builder.ConfigureTestServices(services =>
            {
                services.AddAntiforgery(options => options.SuppressXFrameOptionsHeader = true);

                services.RemoveAll<IAntiforgery>();
            });
        }

        private void SeedTestData(ApplicationDbContext context)
        {
            context.Users.Add(_testUser);

            context.Users.Add(_testSupervisorUser);

            context.SaveChanges();


            context.Requests.AddRange(new List<Request>()
            {
                new Request
                {
                    Id = 1,
                    RequestTypeId = 1,
                    ZoneId = 1,
                    RequestStatusId = 1,
                    IsActive = true,
                    Comments = "Valid test comments.",
                    CreatedBy = _testUser.Id,
                    CreatedDate = DateTime.UtcNow,
                },

                new Request
                {
                    Id = 2,
                    RequestTypeId = 1,
                    ZoneId = 1,
                    RequestStatusId = 1,
                    IsActive = false,
                    Comments = "Inactive request test comments.",
                    CreatedBy = _testUser.Id,
                    CreatedDate = DateTime.UtcNow,
                },
            });
            context.SaveChanges();
        }

        private string GenerateJwtToken(User user,ITokenGenerator tokenGenerator)
        {
            var tokenRequest = new TokenGenerationRequest
            {
                UserId = user.Id,
                Email = user.Email,
                IsSupervisor = user.IsSupervisor,
                IsAdmin = user.IsAdmin,
            };

            return tokenGenerator.GenerateToken(tokenRequest);
        }
    }
}
