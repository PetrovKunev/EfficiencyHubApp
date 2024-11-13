using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfficiencyHub.Services.Data
{
    public class AssignmentService
    {
        private readonly IRepository<Assignment> _assignmentRepository;
        private readonly IRepository<ProjectAssignment> _projectAssignmentRepository;

        public AssignmentService(IRepository<Assignment> assignmentRepository, IRepository<ProjectAssignment> projectAssignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
            _projectAssignmentRepository = projectAssignmentRepository;
        }

        public async Task<IEnumerable<AssignmentViewModel>> GetAssignmentsForProjectAsync(Guid projectId)
        {
            var projectAssignments = await _projectAssignmentRepository.GetAllAsync();
            var assignments = projectAssignments
                .Where(pa => pa.ProjectId == projectId && !pa.Assignment.IsDeleted)
                .Select(pa => new AssignmentViewModel
                {
                    Id = pa.Assignment.Id,
                    Title = pa.Assignment.Title,
                    Description = pa.Assignment.Description,
                    DueDate = pa.Assignment.DueDate,
                    Status = pa.Assignment.Status
                })
                .ToList();

            return assignments;
        }

        public async Task<bool> CreateAssignmentAsync(AssignmentCreateViewModel model, Guid projectId)
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

            await _assignmentRepository.AddAsync(assignment);

            // Link assignment to the project
            var projectAssignment = new ProjectAssignment
            {
                ProjectId = projectId,
                AssignmentId = assignment.Id
            };

            await _projectAssignmentRepository.AddAsync(projectAssignment);
            return true;
        }
    }
}
