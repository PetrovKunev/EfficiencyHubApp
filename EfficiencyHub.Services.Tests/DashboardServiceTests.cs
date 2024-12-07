using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Services.Data;
using Moq;
using System.Linq.Expressions;
using MockQueryable;

namespace EfficiencyHub.Services.Tests
{
    public class DashboardServiceTests
    {
        private readonly Mock<IRepository<Project>> mockProjectRepository;
        private readonly Mock<IRepository<Assignment>> mockAssignmentRepository;
        private readonly Mock<IRepository<ActivityLog>> mockActivityLogRepository;
        private readonly Mock<IRepository<Reminder>> mockReminderRepository;
        private readonly Mock<IRepository<ProjectAssignment>> mockProjectAssignmentRepository;

        public DashboardServiceTests()
        {
            this.mockProjectRepository = new Mock<IRepository<Project>>();
            this.mockAssignmentRepository = new Mock<IRepository<Assignment>>();
            this.mockActivityLogRepository = new Mock<IRepository<ActivityLog>>();
            this.mockReminderRepository = new Mock<IRepository<Reminder>>();
            this.mockProjectAssignmentRepository = new Mock<IRepository<ProjectAssignment>>();
        }

        private DashboardService CreateService()
        {
            return new DashboardService(
                mockProjectRepository.Object,
                mockAssignmentRepository.Object,
                mockActivityLogRepository.Object,
                mockReminderRepository.Object,
                mockProjectAssignmentRepository.Object);
        }

        [Fact]
        public async Task GetDashboardDataAsync_ShouldReturnCorrectData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projectAssignments = new List<ProjectAssignment>
            {
                new ProjectAssignment { UserId = userId, ProjectId = Guid.NewGuid(), AssignmentId = Guid.NewGuid(), IsDeleted = false },
                new ProjectAssignment { UserId = userId, ProjectId = Guid.NewGuid(), AssignmentId = Guid.NewGuid(), IsDeleted = false }
            }.AsQueryable().BuildMock();

            var assignments = new List<Assignment>
            {
                new Assignment { Id = projectAssignments.First().AssignmentId, Status = AssignmentStatus.Completed, IsDeleted = false },
                new Assignment { Id = projectAssignments.Last().AssignmentId, Status = AssignmentStatus.InProgress, IsDeleted = false }
            }.AsQueryable().BuildMock();

            var projects = new List<Project>
            {
                new Project { Id = projectAssignments.First().ProjectId, IsDeleted = false },
                new Project { Id = projectAssignments.Last().ProjectId, IsDeleted = false }
            }.AsQueryable().BuildMock();

            var activityLogs = new List<ActivityLog>
            {
                new ActivityLog { UserId = userId, Timestamp = DateTime.UtcNow },
                new ActivityLog { UserId = userId, Timestamp = DateTime.UtcNow.AddMinutes(-5) }
            }.AsQueryable().BuildMock();

            var reminders = new List<Reminder>
            {
                new Reminder { UserId = userId, ReminderDate = DateTime.UtcNow.AddDays(1), IsDeleted = false },
                new Reminder { UserId = userId, ReminderDate = DateTime.UtcNow.AddDays(2), IsDeleted = false }
            }.AsQueryable().BuildMock();

            mockProjectAssignmentRepository
                .Setup(repo => repo.GetWhereAsync(It.IsAny<Expression<Func<ProjectAssignment, bool>>>()))
                .ReturnsAsync(projectAssignments);

            mockAssignmentRepository
                .Setup(repo => repo.GetWhereAsync(It.IsAny<Expression<Func<Assignment, bool>>>()))
                .ReturnsAsync(assignments);

            mockProjectRepository
                .Setup(repo => repo.GetWhereAsync(It.IsAny<Expression<Func<Project, bool>>>()))
                .ReturnsAsync(projects);

            mockActivityLogRepository
                .Setup(repo => repo.GetWhereAsync(It.IsAny<Expression<Func<ActivityLog, bool>>>()))
                .ReturnsAsync(activityLogs);

            mockReminderRepository
                .Setup(repo => repo.GetWhereAsync(It.IsAny<Expression<Func<Reminder, bool>>>()))
                .ReturnsAsync(reminders);

            var service = CreateService();

            // Act
            var result = await service.GetDashboardDataAsync(userId);

            // Assert
            Assert.Equal(2, result.ProjectCount);
            Assert.Equal(2, result.TaskCount);
            Assert.Equal(1, result.CompletedTaskCount);
            Assert.Equal(2, result.RecentActivityLogs.Count());
            Assert.Equal(2, result.UpcomingReminders.Count());
        }
    }
}
