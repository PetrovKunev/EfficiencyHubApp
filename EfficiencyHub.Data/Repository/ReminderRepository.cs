using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;

namespace EfficiencyHub.Data.Repository
{
    public class ReminderRepository : IRepository<Reminder>
    {
        public Task AddAsync(Reminder entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Reminder>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Reminder> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Reminder entity)
        {
            throw new NotImplementedException();
        }
    }
}
