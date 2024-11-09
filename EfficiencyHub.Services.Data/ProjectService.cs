using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;

namespace EfficiencyHub.Services.Data
{
    public class ProjectService
    {
        private readonly IRepository<Project> _projectRepository;

        public ProjectService(IRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<Project>> GetProjectsForUserAsync(Guid userId)
        {
            var allProjects = await _projectRepository.GetAllAsync();
            return allProjects.Where(p => p.UserId == userId && !p.IsDeleted);
        }

        public async Task<Project> GetProjectByIdAsync(Guid id)
        {
            return await _projectRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateProjectAsync(Project project, Guid userId)
        {
            if (project == null)
            {
                return false;
            }

            project.UserId = userId;
            await _projectRepository.AddAsync(project);
            return true;
        }

        public async Task<bool> UpdateProjectAsync(Project project)
        {
            if (project == null || project.Id == Guid.Empty)
            {
                return false;
            }

            await _projectRepository.UpdateAsync(project);
            return true;
        }

        public async Task<bool> DeleteProjectAsync(Guid id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return false;
            }

            project.IsDeleted = true;
            await _projectRepository.UpdateAsync(project);
            return true;
        }

    }

}
