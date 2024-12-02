using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyHub.Web.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedData(ModelBuilder modelBuilder)
        {
            var adminId = Guid.NewGuid();
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();
            var user3Id = Guid.NewGuid();
            var user4Id = Guid.NewGuid();

            var project1Id = Guid.NewGuid();
            var project2Id = Guid.NewGuid();
            var project3Id = Guid.NewGuid();
            var project4Id = Guid.NewGuid();
            var project5Id = Guid.NewGuid();

            var task1Id = Guid.NewGuid();
            var task2Id = Guid.NewGuid();
            var task3Id = Guid.NewGuid();
            var task4Id = Guid.NewGuid();
            var task5Id = Guid.NewGuid();

            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser { Id = adminId, UserName = "admin", Email = "admin@example.com", EmailConfirmed = true },
                new ApplicationUser { Id = user1Id, UserName = "user1", Email = "user1@example.com", EmailConfirmed = true },
                new ApplicationUser { Id = user2Id, UserName = "user2", Email = "user2@example.com", EmailConfirmed = true },
                new ApplicationUser { Id = user3Id, UserName = "user3", Email = "user3@example.com", EmailConfirmed = true },
                new ApplicationUser { Id = user4Id, UserName = "user4", Email = "user4@example.com", EmailConfirmed = true }
            );

            modelBuilder.Entity<Project>().HasData(
                new Project { Id = project1Id, Name = "Project 1", Description = "Description 1", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), UserId = adminId, Role = ProjectRole.ProjectManager, IsDeleted = false },
                new Project { Id = project2Id, Name = "Project 2", Description = "Description 2", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), UserId = user1Id, Role = ProjectRole.Contributor, IsDeleted = false },
                new Project { Id = project3Id, Name = "Project 3", Description = "Description 3", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), UserId = user2Id, Role = ProjectRole.Contributor, IsDeleted = false },
                new Project { Id = project4Id, Name = "Project 4", Description = "Description 4", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), UserId = user3Id, Role = ProjectRole.ProjectManager, IsDeleted = false },
                new Project { Id = project5Id, Name = "Project 5", Description = "Description 5", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), UserId = user4Id, Role = ProjectRole.Contributor, IsDeleted = false }
            );

            modelBuilder.Entity<Assignment>().HasData(
                new Assignment { Id = task1Id, Title = "Task 1", Description = "Task 1 Description", DueDate = DateTime.Now.AddDays(5), Status = AssignmentStatus.OnHold, CreatedDate = DateTime.Now, IsDeleted = false },
                new Assignment { Id = task2Id, Title = "Task 2", Description = "Task 2 Description", DueDate = DateTime.Now.AddDays(10), Status = AssignmentStatus.InProgress, CreatedDate = DateTime.Now, IsDeleted = false },
                new Assignment { Id = task3Id, Title = "Task 3", Description = "Task 3 Description", DueDate = DateTime.Now.AddDays(15), Status = AssignmentStatus.InProgress, CreatedDate = DateTime.Now, IsDeleted = false },
                new Assignment { Id = task4Id, Title = "Task 4", Description = "Task 4 Description", DueDate = DateTime.Now.AddDays(20), Status = AssignmentStatus.NotStarted, CreatedDate = DateTime.Now, IsDeleted = false },
                new Assignment { Id = task5Id, Title = "Task 5", Description = "Task 5 Description", DueDate = DateTime.Now.AddDays(25), Status = AssignmentStatus.OnHold, CreatedDate = DateTime.Now, IsDeleted = false }
            );

            modelBuilder.Entity<ProjectAssignment>().HasData(
                new ProjectAssignment { ProjectId = project1Id, AssignmentId = task1Id, UserId = adminId, IsDeleted = false },
                new ProjectAssignment { ProjectId = project2Id, AssignmentId = task2Id, UserId = user1Id, IsDeleted = false },
                new ProjectAssignment { ProjectId = project3Id, AssignmentId = task3Id, UserId = user2Id, IsDeleted = false },
                new ProjectAssignment { ProjectId = project4Id, AssignmentId = task4Id, UserId = user3Id, IsDeleted = false },
                new ProjectAssignment { ProjectId = project5Id, AssignmentId = task5Id, UserId = user4Id, IsDeleted = false }
            );

            modelBuilder.Entity<Reminder>().HasData(
                new Reminder { Id = Guid.NewGuid(), Message = "Reminder 1", ReminderDate = DateTime.Now.AddDays(1), AssignmentId = task1Id, UserId = adminId, IsDeleted = false },
                new Reminder { Id = Guid.NewGuid(), Message = "Reminder 2", ReminderDate = DateTime.Now.AddDays(2), AssignmentId = task2Id, UserId = user1Id, IsDeleted = false },
                new Reminder { Id = Guid.NewGuid(), Message = "Reminder 3", ReminderDate = DateTime.Now.AddDays(3), AssignmentId = task3Id, UserId = user2Id, IsDeleted = false },
                new Reminder { Id = Guid.NewGuid(), Message = "Reminder 4", ReminderDate = DateTime.Now.AddDays(4), AssignmentId = task4Id, UserId = user3Id, IsDeleted = false },
                new Reminder { Id = Guid.NewGuid(), Message = "Reminder 5", ReminderDate = DateTime.Now.AddDays(5), AssignmentId = task5Id, UserId = user4Id, IsDeleted = false }
            );

            modelBuilder.Entity<ActivityLog>().HasData(
                new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Created, UserId = adminId, Description = "Admin created a project." },
                new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Updated, UserId = user1Id, Description = "User 1 updated a task." },
                new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Deleted, UserId = user2Id, Description = "User 2 deleted a task." },
                new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Created, UserId = user3Id, Description = "User 3 created a reminder." },
                new ActivityLog { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, ActionType = ActionType.Updated, UserId = user4Id, Description = "User 4 updated a project." }
            );
        }
    }
}