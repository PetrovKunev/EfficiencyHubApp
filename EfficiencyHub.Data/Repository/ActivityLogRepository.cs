using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;

namespace EfficiencyHub.Data.Repository
{
    public class ActivityLogRepository : IRepository<ActivityLog>
    {
        public Task AddAsync(ActivityLog entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ActivityLog>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ActivityLog> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ActivityLog entity)
        {
            throw new NotImplementedException();
        }
    }
}
