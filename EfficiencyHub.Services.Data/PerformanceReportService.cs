using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Web.ViewModels;
using Microsoft.EntityFrameworkCore;


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
                                    a.Status == EfficiencyHub.Common.Enums.AssignmentStatus.Completed &&
                                    a.DueDate >= startDate &&
                                    a.DueDate <= endDate)
            .ToListAsync();

        var completedTaskCount = completedAssignments.Count;

        
        var averageCompletionTime = completedTaskCount > 0
            ? Math.Round(completedAssignments.Average(a => EF.Functions.DateDiffDay(
                a.DueDate.AddDays(-7),
                a.DueDate)), 2)
            : 0;

        
        return new PerformanceReportViewModel
        {
            CompletedTasks = completedTaskCount,
            AverageCompletionTime = averageCompletionTime,
            ReportDate = DateTime.UtcNow
        };
    }
}
