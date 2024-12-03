using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data;
using Microsoft.AspNetCore.Identity;

namespace EfficiencyHub.Web.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var userId = Guid.NewGuid();
                var user = new ApplicationUser
                {
                    Id = userId,
                    UserName = "testuser",
                    Email = "testuser@example.com",
                    EmailConfirmed = true,
                    PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(new ApplicationUser(), "Test@123")
                };

                context.Users.Add(user);
                context.SaveChanges();

                
                Guid project1Id = Guid.NewGuid();
                Guid project2Id = Guid.NewGuid();

                Guid task1Id = Guid.NewGuid();
                Guid task2Id = Guid.NewGuid();

                if (!context.Projects.Any())
                {
                    var projects = new[]
                    {
                        new Project { Id = project1Id, Name = "Project 1", Description = "Description 1", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), UserId = userId, Role = ProjectRole.ProjectManager, IsDeleted = false },
                        new Project { Id = project2Id, Name = "Project 2", Description = "Description 2", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(2), UserId = userId, Role = ProjectRole.Contributor, IsDeleted = false }
                    };
                    context.Projects.AddRange(projects);
                    context.SaveChanges();
                }

                if (!context.Tasks.Any())
                {
                    var tasks = new[]
                    {
                        new Assignment { Id = task1Id, Title = "Task 1", Description = "Task 1 Description", DueDate = DateTime.Now.AddDays(5), Status = AssignmentStatus.InProgress, CreatedDate = DateTime.Now, IsDeleted = false },
                        new Assignment { Id = task2Id, Title = "Task 2", Description = "Task 2 Description", DueDate = DateTime.Now.AddDays(10), Status = AssignmentStatus.Completed, CreatedDate = DateTime.Now, IsDeleted = false }
                    };
                    context.Tasks.AddRange(tasks);
                    context.SaveChanges();
                }

                if (!context.ProjectAssignments.Any())
                {
                    var projectAssignments = new[]
                    {
                        new ProjectAssignment { ProjectId = project1Id, AssignmentId = task1Id, UserId = userId, IsDeleted = false },
                        new ProjectAssignment { ProjectId = project2Id, AssignmentId = task2Id, UserId = userId, IsDeleted = false }
                    };
                    context.ProjectAssignments.AddRange(projectAssignments);
                    context.SaveChanges();
                }



                if (!context.Reminders.Any())
                {
                    var reminders = new[]
                    {
                        new Reminder { Id = Guid.NewGuid(), Message = "Reminder 1", ReminderDate = DateTime.Now.AddDays(2), AssignmentId = context.Tasks.First().Id, UserId = userId, IsDeleted = false },
                        new Reminder { Id = Guid.NewGuid(), Message = "Reminder 2", ReminderDate = DateTime.Now.AddDays(3), AssignmentId = context.Tasks.Last().Id, UserId = userId, IsDeleted = false }
                    };
                    context.Reminders.AddRange(reminders);
                    context.SaveChanges();
                }

                if (!context.ActivityLogs.Any())
                {
                    var logs = new[]
                    {
                        new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Created, UserId = userId, Description = "Created a project" },
                        new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Updated, UserId = userId, Description = "Updated a task" }
                    };
                    context.ActivityLogs.AddRange(logs);
                    context.SaveChanges();
                }
            }
        }

    }

}