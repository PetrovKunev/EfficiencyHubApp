using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

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
            if (endDate < startDate)
            {
                throw new ArgumentException("End date cannot be earlier than start date.");
            }

            
            var completedAssignments = await _assignmentRepository
                .GetQueryableWhere(a => a.ProjectAssignments.Any(pa => pa.UserId == userId) &&
                                        a.CompletedDate.HasValue &&
                                        a.CompletedDate.Value.Date >= startDate.Date &&
                                        a.CompletedDate.Value.Date <= endDate.Date)
                .Select(a => new
                {
                    a.CreatedDate,
                    CompletedDate = a.CompletedDate ?? DateTime.MinValue
                })
                .ToListAsync();

            
            var completedTaskCount = completedAssignments.Count;

            
            var averageCompletionTime = completedTaskCount > 0
                ? Math.Round(completedAssignments
                      .Select(a => (a.CompletedDate - a.CreatedDate).TotalDays)
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