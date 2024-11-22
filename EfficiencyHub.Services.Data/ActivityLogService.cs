using EfficiencyHub.Common.Enums;
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

    public async Task LogActionAsync(Guid userId, ActionType actionType, string description)
    {
        try
        {
            var log = new ActivityLog
            {
                UserId = userId,
                ActionType = actionType,
                Description = description,
                Timestamp = DateTime.UtcNow
            };

            await _activityLogRepository.AddAsync(log);
            _logger.LogInformation("Successfully logged action: {ActionType} by User: {UserId}", actionType, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging action: {ActionType} for User: {UserId}", actionType, userId);
            throw;
        }
    }

}