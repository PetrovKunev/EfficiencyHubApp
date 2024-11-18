using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Web.ViewModels;

namespace EfficiencyHub.Services.Data
{
    public class ReminderService
    {
        private readonly IRepository<Reminder> _reminderRepository;

        public ReminderService(IRepository<Reminder> reminderRepository)
        {
            _reminderRepository = reminderRepository;
        }

        public async Task<IEnumerable<ReminderViewModel>> GetRemindersForUserAsync(Guid userId)
        {
            var reminders = await _reminderRepository.GetWhereAsync(r => r.UserId == userId);
            return reminders.Select(r => new ReminderViewModel
            {
                Id = r.Id,
                Message = r.Message,
                ReminderDate = r.ReminderDate,
                AssignmentId = r.AssignmentId,
                AssignmentTitle = r.Assignment.Title
            });
        }

        public async Task<ReminderEditViewModel?> GetReminderByIdAsync(Guid id)
        {
            var reminder = await _reminderRepository.GetByIdAsync(id);
            if (reminder == null)
            {
                return null;
            }

            return new ReminderEditViewModel
            {
                Id = reminder.Id,
                Message = reminder.Message,
                ReminderDate = reminder.ReminderDate,
                AssignmentId = reminder.AssignmentId,
                AssignmentTitle = reminder.Assignment.Title
            };
        }

        public async Task AddReminderAsync(ReminderCreateViewModel model, Guid userId)
        {
            var reminder = new Reminder
            {
                Id = Guid.NewGuid(),
                Message = model.Message,
                ReminderDate = model.ReminderDate,
                AssignmentId = model.AssignmentId,
                UserId = userId
            };

            await _reminderRepository.AddAsync(reminder);
        }

        public async Task UpdateReminderAsync(ReminderEditViewModel model)
        {
            var reminder = await _reminderRepository.GetByIdAsync(model.Id);
            if (reminder != null)
            {
                reminder.Message = model.Message;
                reminder.ReminderDate = model.ReminderDate;
                await _reminderRepository.UpdateAsync(reminder);
            }
        }

        public async Task<IEnumerable<ReminderViewModel>> GetRemindersByAssignmentAsync(Guid assignmentId, Guid userId)
        {
            var reminders = await _reminderRepository.GetWhereAsync(r => r.AssignmentId == assignmentId && r.UserId == userId);

            return reminders.Select(r => new ReminderViewModel
            {
                Id = r.Id,
                Message = r.Message,
                ReminderDate = r.ReminderDate,
                AssignmentName = r.Assignment.Title
            }).ToList();
        }

        public async Task DeleteReminderAsync(Guid id)
        {
            await _reminderRepository.DeleteAsync(id);
        }
    }

}
