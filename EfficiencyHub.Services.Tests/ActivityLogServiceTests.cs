using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Services.Data;
using Microsoft.Extensions.Logging;
using Moq;
using MockQueryable;
using System.Linq.Expressions;


namespace EfficiencyHub.Services.Tests
{
    public class ActivityLogServiceTests
    {
        private readonly Mock<IRepository<ActivityLog>> mockActivityLogRepository;
        private readonly Mock<IRepository<Assignment>> mockAssignmentRepository;
        private readonly Mock<IRepository<Reminder>> mockReminderRepository;
        private readonly Mock<IRepository<Project>> mockProjectRepository;
        private readonly Mock<IRepository<ProjectAssignment>> mockProjectAssignmentRepository;
        private readonly Mock<ILogger<ActivityLogService>> mockLogger;

        public ActivityLogServiceTests()
        {
            this.mockActivityLogRepository = new Mock<IRepository<ActivityLog>>();
            this.mockAssignmentRepository = new Mock<IRepository<Assignment>>();
            this.mockReminderRepository = new Mock<IRepository<Reminder>>();
            this.mockProjectRepository = new Mock<IRepository<Project>>();
            this.mockProjectAssignmentRepository = new Mock<IRepository<ProjectAssignment>>();
            this.mockLogger = new Mock<ILogger<ActivityLogService>>();
        }

        private ActivityLogService CreateService()
        {
            return new ActivityLogService(
                mockActivityLogRepository.Object,
                mockLogger.Object,
                null,
                mockAssignmentRepository.Object,
                mockReminderRepository.Object,
                mockProjectRepository.Object,
                mockProjectAssignmentRepository.Object);
        }

        [Fact]
        public async Task GetTotalLogsAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var activityLogs = new List<ActivityLog>
            {
                new ActivityLog { UserId = userId, ActionType = ActionType.Created, Description = "Created task 1", Timestamp = DateTime.UtcNow },
                new ActivityLog { UserId = userId, ActionType = ActionType.Updated, Description = "Updated task 2", Timestamp = DateTime.UtcNow }
            }.AsQueryable().BuildMock();

            this.mockActivityLogRepository
                .Setup(repo => repo.GetQueryableWhere(It.IsAny<Expression<Func<ActivityLog, bool>>>()))
                .Returns((Expression<Func<ActivityLog, bool>> predicate) =>
                    activityLogs.Where(predicate.Compile()).AsQueryable());

            var service = CreateService();

            // Act
            var totalLogs = await service.GetTotalLogsAsync(userId);

            // Assert
            Assert.Equal(2, totalLogs);
        }

        [Fact]
        public async Task GetPagedUserActionsAsync_ShouldReturnPagedLogs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var activityLogs = new List<ActivityLog>
            {
                new ActivityLog { UserId = userId, ActionType = ActionType.Created, Description = "Created task 1", Timestamp = DateTime.UtcNow.AddMinutes(-10) },
                new ActivityLog { UserId = userId, ActionType = ActionType.Updated, Description = "Updated task 2", Timestamp = DateTime.UtcNow.AddMinutes(-5) },
                new ActivityLog { UserId = userId, ActionType = ActionType.Deleted, Description = "Deleted task 3", Timestamp = DateTime.UtcNow.AddMinutes(-1) }
            }.AsQueryable().BuildMock();

            this.mockActivityLogRepository
            .Setup(repo => repo.GetQueryableWhere(It.IsAny<Expression<Func<ActivityLog, bool>>>()))
            .Returns((Expression<Func<ActivityLog, bool>> predicate) =>
                activityLogs.Where(predicate.Compile()).AsQueryable());

            var service = CreateService();

            // Act
            var logs = await service.GetPagedUserActionsAsync(userId, pageNumber: 1, pageSize: 2);

            // Assert
            Assert.Equal(2, logs.Count());
            Assert.Equal("Deleted task 3", logs.First().Description);
        }

        [Fact]
        public async Task LogActionAsync_ShouldAddLogToRepository()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var actionType = ActionType.Created;
            var description = "Created a new task";

            this.mockActivityLogRepository
                .Setup(repo => repo.AddAsync(It.IsAny<ActivityLog>()))
                .Returns(Task.CompletedTask);

            var service = CreateService();

            // Act
            await service.LogActionAsync(userId, actionType, description);

            // Assert
            this.mockActivityLogRepository.Verify(repo => repo.AddAsync(It.Is<ActivityLog>(
                log => log.UserId == userId &&
                       log.ActionType == actionType &&
                       log.Description.Contains(description))), Times.Once);
        }

        [Fact]
        public async Task SearchActivityLogsAsync_ShouldReturnFilteredLogs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var searchTerm = "created";
            var activityLogs = new List<ActivityLog>
            {
                new ActivityLog { UserId = userId, ActionType = ActionType.Created, Description = "Created task 1", Timestamp = DateTime.UtcNow },
                new ActivityLog { UserId = userId, ActionType = ActionType.Updated, Description = "Updated task 2", Timestamp = DateTime.UtcNow },
                new ActivityLog { UserId = userId, ActionType = ActionType.Created, Description = "Another created task", Timestamp = DateTime.UtcNow }
            }.AsQueryable().BuildMock();

            this.mockActivityLogRepository
                .Setup(repo => repo.GetQueryableWhere(It.IsAny<Expression<Func<ActivityLog, bool>>>()))
                .Returns((Expression<Func<ActivityLog, bool>> predicate) =>
                    activityLogs.Where(predicate.Compile()).AsQueryable());

            var service = CreateService();

            // Act
            var (filteredLogs, totalCount) = await service.SearchActivityLogsAsync(userId, searchTerm, pageNumber: 1, pageSize: 10);

            // Assert
            Assert.Equal(2, totalCount);
            Assert.Equal(2, filteredLogs.Count());
            Assert.All(filteredLogs, log => Assert.Contains(searchTerm, log.Description.ToLower()));
        }
    }
}
