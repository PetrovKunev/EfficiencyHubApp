using Microsoft.EntityFrameworkCore;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Data;
using System.Linq.Expressions;

public class ActivityLogRepository : IRepository<ActivityLog>
{
    private readonly ApplicationDbContext _context;

    public ActivityLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ActivityLog> GetByIdAsync(Guid id)
    {
        var activityLog = await _context.ActivityLogs.FindAsync(id);
        return activityLog ?? throw new InvalidOperationException("Activity log not found.");
    }

    public async Task<IEnumerable<ActivityLog>> GetAllAsync()
    {
        return await _context.ActivityLogs.ToListAsync();
    }

    public async Task AddAsync(ActivityLog entity)
    {
        await _context.ActivityLogs.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ActivityLog entity)
    {
        _context.ActivityLogs.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var activityLog = await GetByIdAsync(id);
        if (activityLog != null)
        {
            _context.ActivityLogs.Remove(activityLog);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ActivityLog>> GetWhereAsync(Expression<Func<ActivityLog, bool>> predicate)
    {
        return await _context.ActivityLogs.Where(predicate).ToListAsync();
    }
}
