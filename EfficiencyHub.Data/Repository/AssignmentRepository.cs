using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;

namespace EfficiencyHub.Data.Repository
{
    public class AssignmentRepository : IRepository<Assignment>
    {
        public Task AddAsync(Assignment entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Assignment>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Assignment> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Assignment entity)
        {
            throw new NotImplementedException();
        }
    }
}
