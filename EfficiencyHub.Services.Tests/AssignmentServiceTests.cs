using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Services.Data;
using Microsoft.Extensions.Logging;
using Moq;
using MockQueryable.Moq;
using System.Linq.Expressions;
using Xunit;
using EfficiencyHub.Web.ViewModels;
using MockQueryable;

namespace EfficiencyHub.Services.Tests
{
    public class AssignmentServiceTests
    {
        private readonly Mock<IRepository<Assignment>> mockAssignmentRepository;
        private readonly Mock<IRepository<ProjectAssignment>> mockProjectAssignmentRepository;
        private readonly Mock<IRepository<Project>> mockProjectRepository;
        private readonly Mock<IRepository<Reminder>> mockReminderRepository;
        private readonly Mock<ILogger<AssignmentService>> mockLogger;
        private readonly Mock<ActivityLogService> mockActivityLogService;

        public AssignmentServiceTests()
        {
            this.mockAssignmentRepository = new Mock<IRepository<Assignment>>();
            this.mockProjectAssignmentRepository = new Mock<IRepository<ProjectAssignment>>();
            this.mockProjectRepository = new Mock<IRepository<Project>>();
            this.mockReminderRepository = new Mock<IRepository<Reminder>>();
            this.mockLogger = new Mock<ILogger<AssignmentService>>();
            this.mockActivityLogService = new Mock<ActivityLogService>(null!, null!, null!, null!, null!, null!, null!);
        }

        private AssignmentService CreateService()
        {
            return new AssignmentService(
                mockAssignmentRepository.Object,
                mockProjectAssignmentRepository.Object,
                mockProjectRepository.Object,
                mockLogger.Object,
                mockActivityLogService.Object,
                mockReminderRepository.Object);
        }

        [Fact]
        public async Task GetAssignmentsForProjectAsync_ShouldReturnAssignments()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var projectAssignments = new List<ProjectAssignment>
            {
                new ProjectAssignment
                {
                    ProjectId = projectId,
                    Assignment = new Assignment
                    {
                        Id = Guid.NewGuid(),
                        Title = "Test Assignment",
                        Description = "Test Description",
                        DueDate = DateTime.UtcNow,
                        Status = AssignmentStatus.InProgress,
                        IsDeleted = false
                    }
                }
            }.AsQueryable().BuildMock();

            mockProjectAssignmentRepository
                .Setup(repo => repo.GetWhereAsync(It.IsAny<Expression<Func<ProjectAssignment, bool>>>()))
                .ReturnsAsync(projectAssignments);

            var service = CreateService();

            // Act
            var result = await service.GetAssignmentsForProjectAsync(projectId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Assignment", result.First().Title);
        }

        [Fact]
        public async Task GetProjectNameAsync_ShouldReturnProjectName()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project { Id = projectId, Name = "Test Project" };

            mockProjectRepository
                .Setup(repo => repo.GetByIdAsync(projectId))
                .ReturnsAsync(project);

            var service = CreateService();

            // Act
            var result = await service.GetProjectNameAsync(projectId);

            // Assert
            Assert.Equal("Test Project", result);
        }


        [Fact]
        public async Task GetAssignmentByIdAsync_ShouldReturnAssignment()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var projectAssignments = new List<ProjectAssignment>
            {
                new ProjectAssignment
                {
                    ProjectId = projectId,
                    AssignmentId = assignmentId,
                    Assignment = new Assignment
                    {
                        Id = assignmentId,
                        Title = "Test Assignment",
                        Description = "Test Description",
                        DueDate = DateTime.UtcNow,
                        Status = AssignmentStatus.NotStarted,
                        IsDeleted = false
                    }
                }
            }.AsQueryable().BuildMock();

            mockProjectAssignmentRepository
                .Setup(repo => repo.GetWhereAsync(It.IsAny<Expression<Func<ProjectAssignment, bool>>>()))
                .ReturnsAsync(projectAssignments);

            var service = CreateService();

            // Act
            var result = await service.GetAssignmentByIdAsync(projectId, assignmentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Assignment", result.Title);
        }


        [Fact]
        public async Task GetAssignmentDetailsByIdAsync_ShouldReturnAssignmentDetails()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var projectAssignments = new List<ProjectAssignment>
            {
                new ProjectAssignment
                {
                    ProjectId = projectId,
                    AssignmentId = assignmentId,
                    Assignment = new Assignment
                    {
                        Id = assignmentId,
                        Title = "Test Assignment",
                        Description = "Test Description",
                        DueDate = DateTime.UtcNow,
                        Status = AssignmentStatus.InProgress,
                        IsDeleted = false
                    }
                }
            }.AsQueryable().BuildMock();

            mockProjectAssignmentRepository
                .Setup(repo => repo.GetWhereAsync(It.IsAny<Expression<Func<ProjectAssignment, bool>>>()))
                .ReturnsAsync(projectAssignments);

            var service = CreateService();

            // Act
            var result = await service.GetAssignmentDetailsByIdAsync(projectId, assignmentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Assignment", result.Title);
        }

        [Fact]
        public async Task GetAssignmentNameAsync_ShouldReturnAssignmentName()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var assignment = new Assignment { Id = assignmentId, Title = "Test Assignment" };

            mockAssignmentRepository
                .Setup(repo => repo.GetByIdAsync(assignmentId))
                .ReturnsAsync(assignment);

            var service = CreateService();

            // Act
            var result = await service.GetAssignmentNameAsync(assignmentId);

            // Assert
            Assert.Equal("Test Assignment", result);
        }

        [Fact]
        public async Task GetProjectIdByAssignmentAsync_ShouldReturnProjectId()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var projectAssignments = new List<ProjectAssignment>
            {
                new ProjectAssignment
                {
                    AssignmentId = assignmentId,
                    ProjectId = projectId
                }
            }.AsQueryable().BuildMock();

            mockProjectAssignmentRepository
                .Setup(repo => repo.GetWhereAsync(It.IsAny<Expression<Func<ProjectAssignment, bool>>>()))
                .ReturnsAsync(projectAssignments);

            var service = CreateService();

            // Act
            var result = await service.GetProjectIdByAssignmentAsync(assignmentId);

            // Assert
            Assert.Equal(projectId, result);
        }

        [Fact]
        public async Task CreateAssignmentAsync_ShouldReturnFalse_WhenModelIsNull()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var service = CreateService();

            // Act
            var result = await service.CreateAssignmentAsync(null!, projectId, userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CreateAssignmentAsync_ShouldReturnFalse_WhenProjectIdIsEmpty()
        {
            // Arrange
            var model = new AssignmentCreateViewModel
            {
                Title = "New Assignment",
                Description = "New Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = AssignmentStatus.NotStarted
            };
            var userId = Guid.NewGuid();
            var service = CreateService();

            // Act
            var result = await service.CreateAssignmentAsync(model, Guid.Empty, userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAssignmentAsync_ShouldReturnFalse_WhenProjectAssignmentNotFound()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            mockProjectAssignmentRepository
                .Setup(repo => repo.GetQueryableWhere(It.IsAny<Expression<Func<ProjectAssignment, bool>>>()))
                .Returns(Enumerable.Empty<ProjectAssignment>().AsQueryable().BuildMock());

            var service = CreateService();

            // Act
            var result = await service.DeleteAssignmentAsync(projectId, assignmentId, userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAssignmentAsync_ShouldReturnFalse_WhenAssignmentNotFound()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var projectAssignment = new ProjectAssignment
            {
                ProjectId = projectId,
                AssignmentId = assignmentId,
                Assignment = null!
            };

            mockProjectAssignmentRepository
                .Setup(repo => repo.GetQueryableWhere(It.IsAny<Expression<Func<ProjectAssignment, bool>>>()))
                .Returns(new List<ProjectAssignment> { projectAssignment }.AsQueryable().BuildMock());

            var service = CreateService();

            // Act
            var result = await service.DeleteAssignmentAsync(projectId, assignmentId, userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAssignmentAsync_ShouldReturnFalse_WhenModelIsNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var service = CreateService();

            // Act
            var result = await service.UpdateAssignmentAsync(null!, userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAssignmentAsync_ShouldReturnFalse_WhenModelIdIsEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var model = new AssignmentEditViewModel
            {
                Id = Guid.Empty,
                Title = "Updated Assignment",
                Description = "Updated Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = AssignmentStatus.Completed,
                CompletedDate = DateTime.UtcNow
            };
            var service = CreateService();

            // Act
            var result = await service.UpdateAssignmentAsync(model, userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAssignmentAsync_ShouldReturnFalse_WhenProjectAssignmentNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var model = new AssignmentEditViewModel
            {
                Id = Guid.NewGuid(),
                Title = "Updated Assignment",
                Description = "Updated Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = AssignmentStatus.Completed,
                CompletedDate = DateTime.UtcNow
            };

            mockProjectAssignmentRepository
                .Setup(repo => repo.GetWhereAsync(It.IsAny<Expression<Func<ProjectAssignment, bool>>>()))
                .ReturnsAsync(Enumerable.Empty<ProjectAssignment>());

            var service = CreateService();

            // Act
            var result = await service.UpdateAssignmentAsync(model, userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAssignmentAsync_ShouldReturnFalse_WhenAssignmentNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var model = new AssignmentEditViewModel
            {
                Id = Guid.NewGuid(),
                Title = "Updated Assignment",
                Description = "Updated Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = AssignmentStatus.Completed,
                CompletedDate = DateTime.UtcNow
            };
            var projectAssignment = new ProjectAssignment
            {
                AssignmentId = model.Id,
                Assignment = null!
            };

            mockProjectAssignmentRepository
                .Setup(repo => repo.GetWhereAsync(It.IsAny<Expression<Func<ProjectAssignment, bool>>>()))
                .ReturnsAsync(new List<ProjectAssignment> { projectAssignment });

            var service = CreateService();

            // Act
            var result = await service.UpdateAssignmentAsync(model, userId);

            // Assert
            Assert.False(result);
        }
    }
}
