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
            // Fetch completed assignments within the date range
            var completedAssignments = await _assignmentRepository
                .GetQueryableWhere(a => a.ProjectAssignments.Any(pa => pa.UserId == userId) &&
                                        a.CompletedDate.HasValue &&
                                        a.CompletedDate.Value.Date >= startDate.Date &&
                                        a.CompletedDate.Value.Date <= endDate.Date)
                .ToListAsync();

            
            var completedTaskCount = completedAssignments.Count;

            
            var averageCompletionTime = completedTaskCount > 0
                ? Math.Round(completedAssignments
                      .Where(a => a.CompletedDate.HasValue)
                      .Select(a => (a.CompletedDate!.Value - a.CreatedDate).TotalDays)
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