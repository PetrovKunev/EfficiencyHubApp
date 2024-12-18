﻿using EfficiencyHub.Common.Enums;
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
        private readonly ActivityLogService _activityLogService;
        private readonly IRepository<Reminder> _reminderRepository;

        public AssignmentService(
            IRepository<Assignment> assignmentRepository,
            IRepository<ProjectAssignment> projectAssignmentRepository,
            IRepository<Project> projectRepository,
            ILogger<AssignmentService> logger,
            ActivityLogService activityLogService,
            IRepository<Reminder> reminderRepository)
        {
            _assignmentRepository = assignmentRepository;
            _projectAssignmentRepository = projectAssignmentRepository;
            _projectRepository = projectRepository;
            _logger = logger;
            _activityLogService = activityLogService;
            _reminderRepository = reminderRepository;
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
                IsDeleted = false,
                CreatedDate = DateTime.Now
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
                await _activityLogService.LogActionAsync(userId, ActionType.Created, $"Created assignment '{assignment.Title}'", assignment.Id, "Assignment");

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

        public async Task<bool> UpdateAssignmentAsync(AssignmentEditViewModel model, Guid userId)
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

            if (model.Status == AssignmentStatus.Completed)
            {
                assignment.CompletedDate = model.CompletedDate ?? DateTime.Now;
            }

            try
            {
                await _assignmentRepository.UpdateAsync(assignment);

                await _activityLogService.LogActionAsync(userId, ActionType.Updated, $"Updated assignment '{assignment.Title}'", assignment.Id, "Assignment");


                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating assignment {AssignmentId}", model.Id);
                return false;
            }
        }

        public async Task<bool> DeleteAssignmentAsync(Guid projectId, Guid assignmentId, Guid userId)
        {
            var projectAssignment = await _projectAssignmentRepository
                .GetQueryableWhere(pa => pa.ProjectId == projectId && pa.AssignmentId == assignmentId)
                .Include(pa => pa.Assignment)
                .Include(pa => pa.Project)
                .FirstOrDefaultAsync();

            if (projectAssignment == null || projectAssignment.Assignment == null)
            {
                return false;
            }

            var projectName = projectAssignment.Project?.Name ?? "Project not found";

            var assignment = projectAssignment.Assignment;
            assignment.IsDeleted = true;

            try
            {
                var reminders = await _reminderRepository.GetWhereAsync(r => r.AssignmentId == assignment.Id && !r.IsDeleted);
                foreach (var reminder in reminders)
                {
                    reminder.IsDeleted = true;
                    await _reminderRepository.UpdateAsync(reminder);

                    await _activityLogService.LogActionAsync(userId, ActionType.Deleted,
                        $"Deleted reminder with message '{reminder.Message}' (linked to assignment: '{assignment.Title}')",
                        reminder.Id, "Reminder");
                }

                await _assignmentRepository.UpdateAsync(assignment);
                await _projectAssignmentRepository.DeleteEntityAsync(projectAssignment);

                await _activityLogService.LogActionAsync(userId, ActionType.Deleted,
                    $"Deleted assignment '{assignment.Title}' (part of project: '{projectName}')", assignment.Id, "Assignment");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting assignment {AssignmentId} and its reminders", assignmentId);
                return false;
            }
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

        public async Task<IEnumerable<AssignmentViewModel>> GetFilteredAssignmentsAsync(AssignmentFilterViewModel filters, Guid projectId)
        {
            
            var query = _assignmentRepository.GetQueryableWhere(a =>
                a.ProjectAssignments.Any(pa => pa.ProjectId == projectId));

            
            if (!string.IsNullOrEmpty(filters.Title))
            {
                query = query.Where(a => a.Title.Contains(filters.Title));
            }

            if (filters.DueDateFrom.HasValue)
            {
                query = query.Where(a => a.DueDate >= filters.DueDateFrom.Value);
            }

            if (filters.DueDateTo.HasValue)
            {
                query = query.Where(a => a.DueDate <= filters.DueDateTo.Value);
            }

            if (filters.Status.HasValue)
            {
                query = query.Where(a => a.Status == filters.Status.Value);
            }

            
            var assignments = await query.ToListAsync();

            return assignments.Select(a => new AssignmentViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                DueDate = a.DueDate,
                Status = a.Status,
                IsDeleted = a.IsDeleted
            }).ToList();
        }

    }
}
