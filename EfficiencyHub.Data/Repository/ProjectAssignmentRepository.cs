using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.ProjectAssignments.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
