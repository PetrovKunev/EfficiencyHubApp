using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Services.Data;
using EfficiencyHub.Web.ViewModels;
using MockQueryable;
using Moq;
using System.Linq.Expressions;

namespace EfficiencyHub.Services.Tests
{
    public class ProjectServiceTests
    {
        private readonly Mock<IRepository<Project>> mockProjectRepository;
        private readonly Mock<ActivityLogService> mockActivityLogService;

        public ProjectServiceTests()
        {
            this.mockProjectRepository = new Mock<IRepository<Project>>();
            this.mockActivityLogService = new Mock<ActivityLogService>(null!, null!, null!, null!, null!, null!, null!);
        }

        private ProjectService CreateService()
        {
            return new ProjectService(mockProjectRepository.Object, mockActivityLogService.Object);
        }

        [Fact]
        public async Task CreateProjectAsync_ShouldReturnFalse_WhenModelIsNull()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.CreateProjectAsync(null!, Guid.NewGuid());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CreateProjectAsync_ShouldReturnFalse_WhenUserIdIsEmpty()
        {
            // Arrange
            var service = CreateService();
            var model = new ProjectCreateViewModel();

            // Act
            var result = await service.CreateProjectAsync(model, Guid.Empty);

            // Assert
            Assert.False(result);
        }
        

        [Fact]
        public async Task GetProjectsForUserAsync_ShouldReturnProjectsForUser()
        {
            // Arrange
            var service = CreateService();
            var userId = Guid.NewGuid();
            var projects = new List<Project>
            {
                new Project { Id = Guid.NewGuid(), UserId = userId, Name = "Project 1", IsDeleted = false },
                new Project { Id = Guid.NewGuid(), UserId = userId, Name = "Project 2", IsDeleted = false }
            };

            mockProjectRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(projects);

            // Act
            var result = await service.GetProjectsForUserAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetProjectByIdAsync_ShouldReturnProjectDetails_WhenProjectExists()
        {
            // Arrange
            var service = CreateService();
            var userId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                UserId = userId,
                Name = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Role = ProjectRole.Viewer,
                IsDeleted = false
            };

            mockProjectRepository.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(project);

            // Act
            var result = await service.GetProjectByIdAsync(projectId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projectId, result.Id);
        }


        [Fact]
        public async Task UpdateProjectAsync_ShouldReturnFalse_WhenProjectDoesNotExist()
        {
            // Arrange
            var service = CreateService();
            var model = new ProjectEditViewModel { Id = Guid.NewGuid() };
            var userId = Guid.NewGuid();

            var mockProjectRepository = new Mock<IRepository<Project>>();
            var projectId = Guid.NewGuid();
            mockProjectRepository
                .Setup(repo => repo.GetByIdAsync(projectId))
                .ReturnsAsync(new Project { Id = projectId, Name = "Test Project" });
            // Act
            var result = await service.UpdateProjectAsync(model, userId);

            // Assert
            Assert.False(result);
        }



        [Fact]
        public async Task GetProjectNameAsync_ShouldReturnProjectName_WhenProjectExists()
        {
            // Arrange
            var service = CreateService();
            var projectId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                Name = "Test Project"
            };

            mockProjectRepository.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(project);

            // Act
            var result = await service.GetProjectNameAsync(projectId);

            // Assert
            Assert.Equal("Test Project", result);
        }

        [Fact]
        public async Task GetFilteredProjectsAsync_ShouldReturnFilteredProjects()
        {
            // Arrange
            var service = CreateService();
            var userId = Guid.NewGuid();
            var filters = new ProjectFilterViewModel
            {
                Name = "Project",
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(10),
                Status = "Active"
            };
            var projects = new List<Project>
            {
                new Project { Id = Guid.NewGuid(), UserId = userId, Name = "Project 1", StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(5), IsDeleted = false },
                new Project { Id = Guid.NewGuid(), UserId = userId, Name = "Project 2", StartDate = DateTime.UtcNow.AddDays(-15), EndDate = DateTime.UtcNow.AddDays(-5), IsDeleted = false },
                new Project { Id = Guid.NewGuid(), UserId = userId, Name = "Project 3", StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(5), IsDeleted = true }
            }.AsQueryable().BuildMock();

            mockProjectRepository.Setup(repo => repo.GetQueryableWhere(It.IsAny<Expression<Func<Project, bool>>>())).Returns(projects);

            // Act
            var result = await service.GetFilteredProjectsAsync(filters, userId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Project 1", result.First().Name);
        }

        [Fact]
        public async Task GetProjectByIdAsync_ShouldReturnNull_WhenUserIdDoesNotMatch()
        {
            // Arrange
            var service = CreateService();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                UserId = Guid.NewGuid(), // Different userId
                Name = "Test Project",
                IsDeleted = false
            };

            mockProjectRepository.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(project);

            // Act
            var result = await service.GetProjectByIdAsync(projectId, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProjectByIdAsync_ShouldReturnNull_WhenProjectIsDeleted()
        {
            // Arrange
            var service = CreateService();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                UserId = userId,
                Name = "Test Project",
                IsDeleted = true
            };

            mockProjectRepository.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(project);

            // Act
            var result = await service.GetProjectByIdAsync(projectId, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProjectByIdAsync_ShouldReturnProjectDetails_WhenProjectExistsAndUserIdMatches()
        {
            // Arrange
            var service = CreateService();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                UserId = userId,
                Name = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Role = ProjectRole.Viewer,
                IsDeleted = false
            };

            mockProjectRepository.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(project);

            // Act
            var result = await service.GetProjectByIdAsync(projectId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projectId, result.Id);
            Assert.Equal("Test Project", result.Name);
            Assert.Equal("Test Description", result.Description);
            Assert.Equal(project.StartDate, result.StartDate);
            Assert.Equal(project.EndDate, result.EndDate);
            Assert.Equal(ProjectRole.Viewer, result.Role);
            Assert.Equal("Active", result.Status);
        }

        [Fact]
        public async Task DeleteProjectAsync_ShouldReturnFalse_WhenUserIdDoesNotMatch()
        {
            // Arrange
            var service = CreateService();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                UserId = Guid.NewGuid(),
                IsDeleted = false
            };

            mockProjectRepository.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(project);

            // Act
            var result = await service.DeleteProjectAsync(projectId, userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteProjectAsync_ShouldReturnFalse_WhenProjectIsAlreadyDeleted()
        {
            // Arrange
            var service = CreateService();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                UserId = userId,
                Name = "Test Project",
                IsDeleted = true
            };

            mockProjectRepository.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(project);

            // Act
            var result = await service.DeleteProjectAsync(projectId, userId);

            // Assert
            Assert.False(result);
        }

    }
}
