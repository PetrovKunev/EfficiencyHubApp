using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Services.Data;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using Moq;
using NSubstitute;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace EfficiencyHub.Services.Tests
{
    public class PerformanceReportServiceTests
    {
        private readonly Mock<IRepository<Assignment>> mockAssignmentRepository;

        public PerformanceReportServiceTests()
        {
            this.mockAssignmentRepository = new Mock<IRepository<Assignment>>();
        }

        [Fact]
        public async Task GetPerformanceReportAsync_ValidInput_ReturnsCorrectData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 12, 31);

            var assignments = new List<Assignment>
        {
            new Assignment
            {
                Id = Guid.NewGuid(),
                CreatedDate = new DateTime(2023, 1, 10),
                CompletedDate = new DateTime(2023, 1, 20),
                ProjectAssignments = new List<ProjectAssignment>
                {
                    new ProjectAssignment { UserId = userId }
                }
            },
            new Assignment
            {
                Id = Guid.NewGuid(),
                CreatedDate = new DateTime(2023, 2, 1),
                CompletedDate = new DateTime(2023, 2, 15),
                ProjectAssignments = new List<ProjectAssignment>
                {
                    new ProjectAssignment { UserId = userId }
                }
            }
        }.AsQueryable().BuildMock();

            this.mockAssignmentRepository
                .Setup(repo => repo.GetQueryableWhere(It.IsAny<Expression<Func<Assignment, bool>>>()))
                .Returns(assignments);

            var service = new PerformanceReportService(this.mockAssignmentRepository.Object);

            // Act
            var result = await service.GetPerformanceReportAsync(userId, startDate, endDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.CompletedTasks);
            Assert.Equal(12, result.AverageCompletionTime);
        }

        [Fact]
        public async Task GetPerformanceReportAsync_EndDateEarlierThanStartDate_ThrowsArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2022, 12, 31); // Невалидни дати

            var service = new PerformanceReportService(this.mockAssignmentRepository.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.GetPerformanceReportAsync(userId, startDate, endDate));
        }
    }
}
