using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Data;

public class ActivityLogRepository : IRepository<ActivityLog>
{
    private readonly ApplicationDbContext _context;

    public ActivityLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ActivityLog> GetByIdAsync(Guid id)
    {
        return await _context.ActivityLogs.FindAsync(id);
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
}
