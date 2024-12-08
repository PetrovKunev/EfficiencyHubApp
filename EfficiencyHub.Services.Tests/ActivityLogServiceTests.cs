using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Services.Data;
using EfficiencyHub.Services.Tests.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace EfficiencyHub.Services.Tests
{
    public class ActivityLogServiceTests
    {
        private readonly Mock<IRepository<ActivityLog>> _activityLogRepositoryMock;
        private readonly Mock<ILogger<ActivityLogService>> _loggerMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IRepository<Assignment>> _assignmentRepositoryMock;
        private readonly Mock<IRepository<Reminder>> _reminderRepositoryMock;
        private readonly Mock<IRepository<Project>> _projectRepositoryMock;
        private readonly Mock<IRepository<ProjectAssignment>> _projectAssignmentRepositoryMock;
        private readonly ActivityLogService _activityLogService;


        public ActivityLogServiceTests()
        {
            _activityLogRepositoryMock = new Mock<IRepository<ActivityLog>>();
            _userManagerMock = CreateUserManagerMock();
            _assignmentRepositoryMock = new Mock<IRepository<Assignment>>();
            _reminderRepositoryMock = new Mock<IRepository<Reminder>>();
            _projectRepositoryMock = new Mock<IRepository<Project>>();
            _projectAssignmentRepositoryMock = new Mock<IRepository<ProjectAssignment>>();
            _loggerMock = new Mock<ILogger<ActivityLogService>>();

            _activityLogService = new ActivityLogService(
                _activityLogRepositoryMock.Object,
                _loggerMock.Object,
                _userManagerMock.Object,
                _assignmentRepositoryMock.Object,
                _reminderRepositoryMock.Object,
                _projectRepositoryMock.Object,
                _projectAssignmentRepositoryMock.Object
            );
        }


        private Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);

            return userManagerMock;
        }

        [Fact]
        public async Task GetPagedUserActionsAsync_ShouldReturnPagedLogs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var logs = new List<ActivityLog>
            {
                new ActivityLog { UserId = userId, Timestamp = DateTime.Now, ActionType = ActionType.Created, Description = "Created something" },
                new ActivityLog { UserId = userId, Timestamp = DateTime.Now.AddMinutes(-1), ActionType = ActionType.Updated, Description = "Updated something" }
            }.AsQueryable();

            
            var mockQueryable = new Mock<IQueryable<ActivityLog>>();
            mockQueryable.As<IAsyncEnumerable<ActivityLog>>()
                .Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<ActivityLog>(logs.GetEnumerator()));

            mockQueryable.As<IQueryable<ActivityLog>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<ActivityLog>(logs.Provider));

            mockQueryable.As<IQueryable<ActivityLog>>()
                .Setup(m => m.Expression)
                .Returns(logs.Expression);

            mockQueryable.As<IQueryable<ActivityLog>>()
                .Setup(m => m.ElementType)
                .Returns(logs.ElementType);

            mockQueryable.As<IQueryable<ActivityLog>>()
                .Setup(m => m.GetEnumerator())
                .Returns(logs.GetEnumerator());

            _activityLogRepositoryMock
                .Setup(repo => repo.GetQueryableWhere(It.IsAny<Expression<Func<ActivityLog, bool>>>()))
                .Returns(mockQueryable.Object);

            // Act
            var result = await _activityLogService.GetPagedUserActionsAsync(userId, 1, 2);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Created something", result.First().Description);
        }

    }
}