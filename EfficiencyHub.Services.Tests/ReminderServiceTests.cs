using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Services.Data;
using EfficiencyHub.Web.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using MockQueryable;
using System.Linq.Expressions;

namespace EfficiencyHub.Services.Tests
{
    public class ReminderServiceTests
    {
        private readonly Mock<IRepository<Reminder>> mockReminderRepository;
        private readonly Mock<ILogger<ReminderService>> mockLogger;
        private readonly Mock<ActivityLogService> mockActivityLogService;

        public ReminderServiceTests()
        {
            this.mockReminderRepository = new Mock<IRepository<Reminder>>();
            this.mockLogger = new Mock<ILogger<ReminderService>>();
            this.mockActivityLogService = new Mock<ActivityLogService>(null!, null!, null!, null!, null!, null!, null!);
        }

        private ReminderService CreateService()
        {
            return new ReminderService(
                mockReminderRepository.Object,
                mockLogger.Object,
                mockActivityLogService.Object);
        }

        [Fact]
        public async Task GetRemindersForUserAsync_ShouldReturnReminders()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var reminders = new List<Reminder>
            {
                new Reminder { Id = Guid.NewGuid(), UserId = userId, Message = "Reminder 1", ReminderDate = DateTime.UtcNow, AssignmentId = Guid.NewGuid() },
                new Reminder { Id = Guid.NewGuid(), UserId = userId, Message = "Reminder 2", ReminderDate = DateTime.UtcNow, AssignmentId = Guid.NewGuid() }
            }.AsQueryable().BuildMock();

            mockReminderRepository
                .Setup(repo => repo.GetWhereAsync(It.IsAny<Expression<Func<Reminder, bool>>>()))
                .ReturnsAsync(reminders);

            var service = CreateService();

            // Act
            var result = await service.GetRemindersForUserAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddReminderAsync_ShouldAddReminder()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var model = new ReminderCreateViewModel { Message = "New Reminder", ReminderDate = DateTime.UtcNow, AssignmentId = Guid.NewGuid() };

            mockReminderRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Reminder>()))
                .Returns(Task.CompletedTask);

            var service = CreateService();

            // Act
            await service.AddReminderAsync(model, userId);

            // Assert
            mockReminderRepository.Verify(repo => repo.AddAsync(It.Is<Reminder>(r => r.Message == model.Message && r.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task UpdateReminderAsync_ShouldUpdateReminder()
        {
            // Arrange
            var reminderId = Guid.NewGuid();
            var reminder = new Reminder { Id = reminderId, Message = "Old Message", ReminderDate = DateTime.UtcNow, AssignmentId = Guid.NewGuid() };
            var model = new ReminderEditViewModel { Id = reminderId, Message = "Updated Message", ReminderDate = DateTime.UtcNow };

            mockReminderRepository
                .Setup(repo => repo.GetByIdAsync(reminderId))
                .ReturnsAsync(reminder);

            mockReminderRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<Reminder>()))
                .Returns(Task.CompletedTask);

            var service = CreateService();

            // Act
            await service.UpdateReminderAsync(model);

            // Assert
            mockReminderRepository.Verify(repo => repo.UpdateAsync(It.Is<Reminder>(r => r.Message == model.Message)), Times.Once);
        }

    }
}
