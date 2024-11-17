using Microsoft.EntityFrameworkCore;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using System.Linq.Expressions;

namespace EfficiencyHub.Data.Repository
{
    public class AssignmentRepository : IRepository<Assignment>
    {
        private readonly ApplicationDbContext _context;

        public AssignmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Assignment> GetByIdAsync(Guid id)
        {
            var result = await _context.Tasks.FindAsync(id);
            return result ?? new Assignment();
        }

        public async Task<IEnumerable<Assignment>> GetAllAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task AddAsync(Assignment entity)
        {
            await _context.Tasks.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Assignment entity)
        {
            _context.Tasks.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var assignment = await GetByIdAsync(id);
            if (assignment != null)
            {
                _context.Tasks.Remove(assignment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Assignment>> GetWhereAsync(Expression<Func<Assignment, bool>> predicate)
        {
            return await _context.Tasks
                .Where(predicate)
                .ToListAsync();
        }

        public IQueryable<Assignment> GetQueryableWhere(Expression<Func<Assignment, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task DeleteEntityAsync(Assignment entity)
        {
            throw new NotImplementedException();
        }
    }
}
