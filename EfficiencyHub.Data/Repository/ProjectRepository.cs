using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Data;
using EfficiencyHub.Data.Models;

public class ProjectRepository : IRepository<Project>
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Project entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Project>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Project> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Project entity)
    {
        throw new NotImplementedException();
    }
}
