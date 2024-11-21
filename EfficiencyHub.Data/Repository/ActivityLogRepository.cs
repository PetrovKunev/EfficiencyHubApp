using EfficiencyHub.Data;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


public class ActivityLogRepository : IRepository<ActivityLog>
{
    private readonly ApplicationDbContext _context;

    public ActivityLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ActivityLog>> GetLastUserActionsAsync(Guid userId, int count = 10)
    {
        return await _context.Set<ActivityLog>()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<ActivityLog> GetByIdAsync(Guid id)
    {
        return await _context.Set<ActivityLog>().FindAsync(id);
    }

    public async Task<IEnumerable<ActivityLog>> GetAllAsync()
    {
        return await _context.Set<ActivityLog>().ToListAsync();
    }

    public async Task<IEnumerable<ActivityLog>> GetWhereAsync(Expression<Func<ActivityLog, bool>> predicate)
    {
        return await _context.Set<ActivityLog>().Where(predicate).ToListAsync();
    }

    public IQueryable<ActivityLog> GetQueryableWhere(Expression<Func<ActivityLog, bool>> predicate)
    {
        return _context.Set<ActivityLog>().Where(predicate);
    }

    public async Task AddAsync(ActivityLog entity)
    {
        await _context.Set<ActivityLog>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ActivityLog entity)
    {
        _context.Set<ActivityLog>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Set<ActivityLog>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteEntityAsync(ActivityLog entity)
    {
        _context.Set<ActivityLog>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}
