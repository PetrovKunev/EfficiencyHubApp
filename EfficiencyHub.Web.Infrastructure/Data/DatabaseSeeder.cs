using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data;

namespace EfficiencyHub.Web.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            
            if (!context.Users.Any())
            {
                var user1Id = Guid.NewGuid();
                var user2Id = Guid.NewGuid();
                var user3Id = Guid.NewGuid();
                var user4Id = Guid.NewGuid();

                var users = new[]
                {
                    new ApplicationUser { Id = user1Id, UserName = "user1", Email = "user1@example.com", EmailConfirmed = true },
                    new ApplicationUser { Id = user2Id, UserName = "user2", Email = "user2@example.com", EmailConfirmed = true },
                    new ApplicationUser { Id = user3Id, UserName = "user3", Email = "user3@example.com", EmailConfirmed = true },
                    new ApplicationUser { Id = user4Id, UserName = "user4", Email = "user4@example.com", EmailConfirmed = true }
                };

                context.Users.AddRange(users);
            }

            
            if (!context.Projects.Any())
            {
                var project1Id = Guid.NewGuid();
                var project2Id = Guid.NewGuid();

                var projects = new[]
                {
                    new Project { Id = project1Id, Name = "Project 1", Description = "Description 1", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), UserId = context.Users.First().Id, Role = ProjectRole.Contributor, IsDeleted = false },
                    new Project { Id = project2Id, Name = "Project 2", Description = "Description 2", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), UserId = context.Users.First().Id, Role = ProjectRole.ProjectManager, IsDeleted = false },
                    new Project { Id = Guid.NewGuid(), Name = "Project 3", Description = "Description 3", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), UserId = context.Users.First().Id, Role = ProjectRole.Contributor, IsDeleted = false }
                };

                context.Projects.AddRange(projects);
            }

           
            if (!context.Tasks.Any())
            {
                var tasks = new[]
                {
                    new Assignment { Id = Guid.NewGuid(), Title = "Task 1", Description = "Task 1 Description", DueDate = DateTime.Now.AddDays(5), Status = AssignmentStatus.OnHold, CreatedDate = DateTime.Now, IsDeleted = false },
                    new Assignment { Id = Guid.NewGuid(), Title = "Task 2", Description = "Task 2 Description", DueDate = DateTime.Now.AddDays(10), Status = AssignmentStatus.InProgress, CreatedDate = DateTime.Now, IsDeleted = false },
                    new Assignment { Id = Guid.NewGuid(), Title = "Task 3", Description = "Task 3 Description", DueDate = DateTime.Now.AddDays(15), Status = AssignmentStatus.Completed, CreatedDate = DateTime.Now, IsDeleted = false },
                    new Assignment { Id = Guid.NewGuid(), Title = "Task 4", Description = "Task 4 Description", DueDate = DateTime.Now.AddDays(20), Status = AssignmentStatus.OnHold, CreatedDate = DateTime.Now, IsDeleted = false },
                    new Assignment { Id = Guid.NewGuid(), Title = "Task 5", Description = "Task 5 Description", DueDate = DateTime.Now.AddDays(25), Status = AssignmentStatus.InProgress, CreatedDate = DateTime.Now, IsDeleted = false }
                };

                context.Tasks.AddRange(tasks);
            }
       
            if (!context.Reminders.Any())
            {
                var reminders = new[]
                {
                    new Reminder { Id = Guid.NewGuid(), Message = "Reminder 1", ReminderDate = DateTime.Now.AddDays(3), AssignmentId = context.Tasks.First().Id, UserId = context.Users.First().Id, IsDeleted = false },
                    new Reminder { Id = Guid.NewGuid(), Message = "Reminder 2", ReminderDate = DateTime.Now.AddDays(4), AssignmentId = context.Tasks.First().Id, UserId = context.Users.First().Id, IsDeleted = false },
                    new Reminder { Id = Guid.NewGuid(), Message = "Reminder 3", ReminderDate = DateTime.Now.AddDays(5), AssignmentId = context.Tasks.First().Id, UserId = context.Users.First().Id, IsDeleted = false },
                    new Reminder { Id = Guid.NewGuid(), Message = "Reminder 4", ReminderDate = DateTime.Now.AddDays(6), AssignmentId = context.Tasks.First().Id, UserId = context.Users.First().Id, IsDeleted = false },
                    new Reminder { Id = Guid.NewGuid(), Message = "Reminder 5", ReminderDate = DateTime.Now.AddDays(7), AssignmentId = context.Tasks.First().Id, UserId = context.Users.First().Id, IsDeleted = false }
                };

                context.Reminders.AddRange(reminders);
            }
       
            if (!context.ActivityLogs.Any())
            {
                var logs = new[]
                {
                    new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Created, UserId = context.Users.First().Id, Description = "Created a task." },
                    new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Updated, UserId = context.Users.First().Id, Description = "Updated a task." },
                    new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Deleted, UserId = context.Users.First().Id, Description = "Deleted a task." },
                    new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Created, UserId = context.Users.First().Id, Description = "Created a project." },
                    new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Updated, UserId = context.Users.First().Id, Description = "Updated a project." },
                    new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Deleted, UserId = context.Users.First().Id, Description = "Deleted a project." }, 
                    new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Created, UserId = context.Users.First().Id, Description = "Created a reminder." },
                    new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Updated, UserId = context.Users.First().Id, Description = "Updated a reminder." },
                    new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Deleted, UserId = context.Users.First().Id, Description = "Deleted a reminder." }

                };

                context.ActivityLogs.AddRange(logs);
            }

            context.SaveChanges();
        }
    }
}