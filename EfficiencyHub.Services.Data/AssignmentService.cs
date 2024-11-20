using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace EfficiencyHub.Services.Data
{
    public class AssignmentService
    {
        private readonly IRepository<Assignment> _assignmentRepository;
        private readonly IRepository<ProjectAssignment> _projectAssignmentRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly ILogger<AssignmentService> _logger;

        public AssignmentService(
            IRepository<Assignment> assignmentRepository,
            IRepository<ProjectAssignment> projectAssignmentRepository,
            IRepository<Project> projectRepository,
            ILogger<AssignmentService> logger)
        {
            _assignmentRepository = assignmentRepository;
            _projectAssignmentRepository = projectAssignmentRepository;
            _projectRepository = projectRepository;
            _logger = logger;
        }


        public async Task<IEnumerable<AssignmentViewModel>> GetAssignmentsForProjectAsync(Guid projectId)
        {
            var projectAssignments = await _projectAssignmentRepository.GetWhereAsync(pa => pa.ProjectId == projectId && !pa.Assignment.IsDeleted);

            return projectAssignments.Select(pa => new AssignmentViewModel
            {
                Id = pa.Assignment.Id,
                Title = pa.Assignment.Title,
                Description = pa.Assignment.Description,
                DueDate = pa.Assignment.DueDate,
                Status = pa.Assignment.Status,
                IsDeleted = pa.Assignment.IsDeleted
            }).ToList();
        }

        public async Task<string> GetProjectNameAsync(Guid projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            return project?.Name ?? "Project";
        }

        public async Task<bool> CreateAssignmentAsync(AssignmentCreateViewModel model, Guid projectId, Guid userId)
        {
            if (model == null || projectId == Guid.Empty)
            {
                return false;
            }

            var assignment = new Assignment
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                Status = model.Status,
                IsDeleted = false
            };

            try
            {
                await _assignmentRepository.AddAsync(assignment);

                var projectAssignment = new ProjectAssignment
                {
                    ProjectId = projectId,
                    AssignmentId = assignment.Id,
                    UserId = userId
                };

                await _projectAssignmentRepository.AddAsync(projectAssignment);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<AssignmentEditViewModel?> GetAssignmentByIdAsync(Guid projectId, Guid assignmentId)
        {
            var projectAssignments = await _projectAssignmentRepository
                .GetWhereAsync(pa => pa.ProjectId == projectId && pa.AssignmentId == assignmentId);

            var projectAssignment = projectAssignments.FirstOrDefault();

            if (projectAssignment == null || projectAssignment.Assignment.IsDeleted)
            {
                return null;
            }

            return new AssignmentEditViewModel
            {
                Id = projectAssignment.Assignment.Id,
                Title = projectAssignment.Assignment.Title,
                Description = projectAssignment.Assignment.Description,
                DueDate = projectAssignment.Assignment.DueDate,
                Status = projectAssignment.Assignment.Status,
                ProjectId = projectAssignment.ProjectId
            };
        }


        public async Task<bool> UpdateAssignmentAsync(AssignmentEditViewModel model)
        {
            if (model == null || model.Id == Guid.Empty)
            {
                return false;
            }

            var projectAssignment = await _projectAssignmentRepository
                .GetWhereAsync(pa => pa.AssignmentId == model.Id);

            if (!projectAssignment.Any())
            {
                return false;
            }

            var assignment = projectAssignment.First().Assignment;
            if (assignment == null)
            {
                return false;
            }

            assignment.Title = model.Title;
            assignment.Description = model.Description;
            assignment.DueDate = model.DueDate;
            assignment.Status = model.Status;

            try
            {
                await _assignmentRepository.UpdateAsync(assignment);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        
        public async Task<bool> DeleteAssignmentAsync(Guid projectId, Guid assignmentId)
        {
            // Намерете връзката в ProjectAssignments
            var projectAssignment = await _projectAssignmentRepository
                .GetQueryableWhere(pa => pa.ProjectId == projectId && pa.AssignmentId == assignmentId)
                .FirstOrDefaultAsync();

            if (projectAssignment == null)
            {
                return false;
            }

            var assignment = projectAssignment.Assignment;
            assignment.IsDeleted = true;

            await _projectAssignmentRepository.DeleteEntityAsync(projectAssignment);

            await _assignmentRepository.UpdateAsync(assignment);

            return true;
        }

        public async Task<AssignmentViewModel?> GetAssignmentDetailsByIdAsync(Guid projectId, Guid assignmentId)
        {
            var projectAssignments = await _projectAssignmentRepository
                .GetWhereAsync(pa => pa.ProjectId == projectId && pa.AssignmentId == assignmentId);

            var projectAssignment = projectAssignments.FirstOrDefault();

            if (projectAssignment == null || projectAssignment.Assignment.IsDeleted)
            {
                return null;
            }

            return new AssignmentViewModel
            {
                Id = projectAssignment.Assignment.Id,
                Title = projectAssignment.Assignment.Title,
                Description = projectAssignment.Assignment.Description,
                DueDate = projectAssignment.Assignment.DueDate,
                Status = projectAssignment.Assignment.Status,
                IsDeleted = projectAssignment.Assignment.IsDeleted
            };
        }

        public async Task<string> GetAssignmentNameAsync(Guid assignmentId)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(assignmentId);
            if (assignment == null)
            {
                throw new KeyNotFoundException("Assignment not found.");
            }

            return assignment.Title;
        }

        public async Task<Guid> GetProjectIdByAssignmentAsync(Guid assignmentId)
        {
            var projectAssignments = await _projectAssignmentRepository
                .GetWhereAsync(pa => pa.AssignmentId == assignmentId);

            if (!projectAssignments.Any())
            {
                _logger.LogError($"No project assignment found for AssignmentId: {assignmentId}");
                throw new InvalidOperationException("Project not found for the given assignment.");
            }

            var projectAssignment = projectAssignments.FirstOrDefault();

            if (projectAssignment == null)
            {
                _logger.LogError($"No valid project assignment found for AssignmentId: {assignmentId}");
                throw new InvalidOperationException("No project found for the specified assignment.");
            }

            _logger.LogInformation($"Found ProjectId: {projectAssignment.ProjectId} for AssignmentId: {assignmentId}");
            return projectAssignment.ProjectId;
        }


    }
}
