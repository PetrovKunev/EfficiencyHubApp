using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EfficiencyHub.Data.Repository
{
    public class ProjectAssignmentRepository : IRepository<ProjectAssignment>
    {
        private readonly ApplicationDbContext _context;

        public ProjectAssignmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectAssignment> GetByIdAsync(Guid id)
        {
            return await _context.ProjectAssignments.FindAsync(id) ?? throw new InvalidOperationException("Project assignment not found.");
        }

        public async Task<IEnumerable<ProjectAssignment>> GetAllAsync()
        {
            return await _context.ProjectAssignments.ToListAsync();
        }

        public async Task AddAsync(ProjectAssignment entity)
        {
            await _context.ProjectAssignments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectAssignment entity)
        {
            _context.ProjectAssignments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProjectAssignment>> GetWhereAsync(Expression<Func<ProjectAssignment, bool>> predicate)
        {
            return await _context.ProjectAssignments
                .Include(pa => pa.Assignment)
                .Include(pa => pa.Project)
                .Where(pa => !pa.IsDeleted)
                .Where(predicate)
                .ToListAsync();
        }

        public IQueryable<ProjectAssignment> GetQueryableWhere(Expression<Func<ProjectAssignment, bool>> predicate)
        {
            return _context.ProjectAssignments
                .Include(pa => pa.Assignment)
                .Include(pa => pa.Project)
                .Where(pa => !pa.IsDeleted)
                .Where(predicate);
        }

        public async Task DeleteEntityAsync(ProjectAssignment entity)
        {
            if (entity != null)
            {
                entity.IsDeleted = true;
                _context.ProjectAssignments.Update(entity);
                await _context.SaveChangesAsync();
            }
        }


        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

    }
}
