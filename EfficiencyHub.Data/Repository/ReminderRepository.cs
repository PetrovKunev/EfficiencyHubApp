using Microsoft.EntityFrameworkCore;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using System.Linq.Expressions;

namespace EfficiencyHub.Data.Repository
{
    public class ReminderRepository : IRepository<Reminder>
    {
        private readonly ApplicationDbContext _context;

        public ReminderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Reminder> GetByIdAsync(Guid id)
        {
            var result = await _context.Reminders.FindAsync(id);
            return result ?? new Reminder();
        }


        public async Task<IEnumerable<Reminder>> GetAllAsync()
        {
            return await _context.Reminders.ToListAsync();
        }

        public async Task AddAsync(Reminder entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await _context.Reminders.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reminder entity)
        {
            _context.Reminders.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var reminder = await GetByIdAsync(id);
            if (reminder != null)
            {
                _context.Reminders.Remove(reminder);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Reminder>> GetWhereAsync(Expression<Func<Reminder, bool>> predicate)
        {
            
            return await _context.Reminders
                .Where(r => !r.IsDeleted)
                .Where(predicate)
                .ToListAsync();
        }

        public IQueryable<Reminder> GetQueryableWhere(Expression<Func<Reminder, bool>> predicate)
        {
            // Не филтрираме по IsDeleted, за да се включат всички напомняния
            return _context.Reminders.Where(predicate);
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var reminder = await GetByIdAsync(id);
            if (reminder != null)
            {
                reminder.IsDeleted = true;
                _context.Reminders.Update(reminder);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteEntityAsync(Reminder entity)
        {
            entity.IsDeleted = true;
            _context.Reminders.Update(entity);
            await _context.SaveChangesAsync();
        }

    }
}
