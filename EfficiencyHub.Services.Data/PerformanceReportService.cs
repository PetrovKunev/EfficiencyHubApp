using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using EfficiencyHub.Common.Enums;

namespace EfficiencyHub.Services.Data
{
    public class PerformanceReportService
    {
        private readonly IRepository<Assignment> _assignmentRepository;

        public PerformanceReportService(IRepository<Assignment> assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public async Task<PerformanceReportViewModel> GetPerformanceReportAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            var completedAssignments = await _assignmentRepository
                .GetQueryableWhere(a => a.ProjectAssignments.Any(pa => pa.UserId == userId) &&
                                        a.Status == AssignmentStatus.Completed &&
                                        a.CompletedDate.HasValue &&
                                        a.CompletedDate.Value >= startDate &&
                                        a.CompletedDate.Value <= endDate)
                .ToListAsync();

            var completedTaskCount = completedAssignments.Count;

            var averageCompletionTime = completedTaskCount > 0
                ? Math.Round(completedAssignments
                    .Select(a => (a.CompletedDate.Value - a.CreatedDate).TotalDays)
                    .Average(), 2)
                : 0;

            return new PerformanceReportViewModel
            {
                CompletedTasks = completedTaskCount,
                AverageCompletionTime = averageCompletionTime,
                ReportDate = DateTime.UtcNow
            };
        }

    }
}