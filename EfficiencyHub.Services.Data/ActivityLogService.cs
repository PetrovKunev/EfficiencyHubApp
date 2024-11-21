using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Web.ViewModels;
using Microsoft.Extensions.Logging;


public class ActivityLogService
{
    private readonly IRepository<ActivityLog> _activityLogRepository;
    private readonly ILogger<ActivityLogService> _logger;

    public ActivityLogService(IRepository<ActivityLog> activityLogRepository, ILogger<ActivityLogService> logger)
    {
        _activityLogRepository = activityLogRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ActivityLogViewModel>> GetLastUserActionsAsync(Guid userId, int count = 10)
    {
        try
        {
            var logs = await _activityLogRepository.GetWhereAsync(a => a.UserId == userId);
            return logs.OrderByDescending(log => log.Timestamp)
                       .Take(count)
                       .Select(log => new ActivityLogViewModel
                       {
                           Timestamp = log.Timestamp,
                           ActionType = log.ActionType.ToString(),
                           Description = log.Description
                       }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user activity logs.");
            throw;
        }
    }
}