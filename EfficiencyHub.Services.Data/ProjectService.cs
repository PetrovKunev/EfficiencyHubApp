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
    }
}
