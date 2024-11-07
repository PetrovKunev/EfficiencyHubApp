using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Data;

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
}
