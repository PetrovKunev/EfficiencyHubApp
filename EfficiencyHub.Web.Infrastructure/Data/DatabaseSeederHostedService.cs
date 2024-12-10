using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data;
using EfficiencyHub.Data.Models;
using Microsoft.AspNetCore.Identity;
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
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            try
            {
                _logger.LogInformation("Starting database seeding...");

                context.Database.Migrate();

                await SeedTestUserAndDataAsync(context, userManager);

                _logger.LogInformation("Database seeding completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database seeding.");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private static async Task SeedTestUserAndDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            var testUserEmail = "testuser@example.com";
            var testUserPassword = "Test@123";

            var existingUser = await userManager.FindByEmailAsync(testUserEmail);
            if (existingUser == null)
            {
                var testUser = new ApplicationUser
                {
                    UserName = testUserEmail,
                    Email = testUserEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(testUser, testUserPassword);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                await userManager.AddToRoleAsync(testUser, "User");

                Guid project1Id = Guid.NewGuid();
                Guid project2Id = Guid.NewGuid();
                Guid task1Id = Guid.NewGuid();
                Guid task2Id = Guid.NewGuid();

                // Projects
                var projects = new[]
                {
                    new Project { Id = project1Id, Name = "Project 1", Description = "Description 1", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), UserId = testUser.Id, Role = ProjectRole.ProjectManager, IsDeleted = false },
                    new Project { Id = project2Id, Name = "Project 2", Description = "Description 2", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(2), UserId = testUser.Id, Role = ProjectRole.Contributor, IsDeleted = false }
                };
                context.Projects.AddRange(projects);

                // Tasks
                var tasks = new[]
                {
                    new Assignment { Id = task1Id, Title = "Task 1", Description = "Task 1 Description", DueDate = DateTime.Now.AddDays(5), Status = AssignmentStatus.InProgress, CreatedDate = DateTime.Now, IsDeleted = false },
                    new Assignment { Id = task2Id, Title = "Task 2", Description = "Task 2 Description", DueDate = DateTime.Now.AddDays(10), Status = AssignmentStatus.Completed, CreatedDate = DateTime.Now, IsDeleted = false }
                };
                context.Tasks.AddRange(tasks);

                // Project Assignments
                var projectAssignments = new[]
                {
                    new ProjectAssignment { ProjectId = project1Id, AssignmentId = task1Id, UserId = testUser.Id, IsDeleted = false },
                    new ProjectAssignment { ProjectId = project2Id, AssignmentId = task2Id, UserId = testUser.Id, IsDeleted = false }
                };
                context.ProjectAssignments.AddRange(projectAssignments);

                // Reminders
                var reminders = new[]
                {
                    new Reminder { Id = Guid.NewGuid(), Message = "Reminder 1", ReminderDate = DateTime.Now.AddDays(2), AssignmentId = task1Id, UserId = testUser.Id, IsDeleted = false },
                    new Reminder { Id = Guid.NewGuid(), Message = "Reminder 2", ReminderDate = DateTime.Now.AddDays(3), AssignmentId = task2Id, UserId = testUser.Id, IsDeleted = false }
                };
                context.Reminders.AddRange(reminders);

                // Activity logs
                var logs = new List<ActivityLog>();

                
                logs.AddRange(new[]
                {
                    new ActivityLog
                    {
                        Id = Guid.NewGuid(),
                        Timestamp = DateTime.UtcNow,
                        ActionType = ActionType.Created,
                        UserId = testUser.Id,
                        Description = "Created a project"
                    },
                    new ActivityLog
                    {
                        Id = Guid.NewGuid(),
                        Timestamp = DateTime.UtcNow,
                        ActionType = ActionType.Updated,
                        UserId = testUser.Id,
                        Description = "Updated a task"
                    }
                });

                
                logs.AddRange(Enumerable.Range(1, 23).Select(i => new ActivityLog
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.UtcNow.AddMinutes(-i),
                    ActionType = i % 2 == 0 ? ActionType.Created : ActionType.Deleted,
                    UserId = testUser.Id,
                    Description = $"Log entry {i}"
                }));

                
                context.ActivityLogs.AddRange(logs);
                await context.SaveChangesAsync();

            }
        }
    }
}
