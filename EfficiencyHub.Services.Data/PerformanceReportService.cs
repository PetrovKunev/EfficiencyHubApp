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

            var completedAssignmentsQuery = _assignmentRepository
                .GetQueryableWhere(a => a.ProjectAssignments.Any(pa => pa.UserId == userId) &&
                                        a.CompletedDate.HasValue &&
                                        a.CompletedDate.Value.Date >= startDate.Date &&
                                        a.CompletedDate.Value.Date <= endDate.Date);

            var completedTaskCount = await completedAssignmentsQuery.CountAsync();

            var averageCompletionTime = completedTaskCount > 0
                ? Math.Round(await completedAssignmentsQuery
                      .Select(a => (a.CompletedDate!.Value - a.CreatedDate).TotalDays)
                      .AverageAsync(), 2)
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