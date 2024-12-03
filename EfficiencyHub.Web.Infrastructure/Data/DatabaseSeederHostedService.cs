using EfficiencyHub.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EfficiencyHub.Web.Infrastructure.Data
{
    public class DatabaseSeederHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseSeederHostedService> _logger;

        public DatabaseSeederHostedService(IServiceProvider serviceProvider, ILogger<DatabaseSeederHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                _logger.LogInformation("Starting database seeding...");

                context.Database.Migrate();

                DatabaseSeeder.SeedData(context);

                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Database seeding completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database seeding.");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
