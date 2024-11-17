using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Web.ViewModels;

namespace EfficiencyHub.Services.Data
{
    public class ProjectService
    {
        private readonly IRepository<Project> _projectRepository;

        public ProjectService(IRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<bool> CreateProjectAsync(ProjectCreateViewModel model, Guid userId)
        {
            if (model == null || userId == Guid.Empty)
            {
                return false;
            }

            var project = new Project
            {
                Name = model.Name,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Role = model.Role,
                UserId = userId,
                IsDeleted = false
            };

            await _projectRepository.AddAsync(project);
            return true;
        }

        public async Task<IEnumerable<ProjectViewModel>> GetProjectsForUserAsync(Guid userId)
        {
            var projects = await _projectRepository.GetAllAsync();
            return projects
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .Select(p => new ProjectViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Role = p.Role,
                    IsDeleted = p.IsDeleted
                }).ToList();
        }

        public async Task<ProjectDetailsViewModel?> GetProjectByIdAsync(Guid projectId, Guid userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.UserId != userId || project.IsDeleted)
            {
                return null;
            }

            return new ProjectDetailsViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Role = project.Role,
                Status = project.IsDeleted ? "Deleted" : "Active"
            };
        }

        public async Task<ProjectEditViewModel?> GetProjectForEditAsync(Guid projectId, Guid userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.UserId != userId || project.IsDeleted)
            {
                return null;
            }

            return new ProjectEditViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Role = project.Role
            };
        }

        public async Task<bool> UpdateProjectAsync(ProjectEditViewModel model, Guid userId)
        {
            var project = await _projectRepository.GetByIdAsync(model.Id);
            if (project == null || project.UserId != userId || project.IsDeleted)
            {
                return false;
            }

            project.Name = model.Name;
            project.Description = model.Description;
            project.StartDate = model.StartDate;
            project.EndDate = model.EndDate;
            project.Role = model.Role;

            await _projectRepository.UpdateAsync(project);
            return true;
        }

        public async Task<bool> DeleteProjectAsync(Guid projectId, Guid userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.UserId != userId || project.IsDeleted)
            {
                return false;
            }

            project.IsDeleted = true;
            await _projectRepository.UpdateAsync(project);
            return true;
        }

        public async Task<dynamic> GetProjectNameAsync(Guid projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            return project?.Name ?? "Unknown Project";
        }
    }
}
