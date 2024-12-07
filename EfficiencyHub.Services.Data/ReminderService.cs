using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfficiencyHub.Services.Data
{
    public class ReminderService
    {
        private readonly IRepository<Reminder> _reminderRepository;
        private readonly ILogger<ReminderService> _logger;
        private readonly ActivityLogService _activityLogService;
        public ReminderService(IRepository<Reminder> reminderRepository, ILogger<ReminderService> logger, ActivityLogService activityLogService)
        {
            _reminderRepository = reminderRepository;
            _logger = logger;
            _activityLogService = activityLogService;
        }

        public async Task<IEnumerable<ReminderViewModel>> GetRemindersForUserAsync(Guid userId)
        {
            
            var reminders = await _reminderRepository.GetWhereAsync(r => r.UserId == userId && !r.IsDeleted);

            return reminders.Select(r => new ReminderViewModel
            {
                Id = r.Id,
                Message = r.Message,
                ReminderDate = r.ReminderDate,
                AssignmentId = r.AssignmentId,
                AssignmentTitle = r.Assignment?.Title ?? "Unknown Assignment"
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
            var reminders = await _reminderRepository.GetWhereAsync(r => r.AssignmentId == assignmentId && r.UserId == userId && !r.IsDeleted);

            return reminders.Select(r => new ReminderViewModel
            {
                Id = r.Id,
                Message = r.Message,
                ReminderDate = r.ReminderDate,
                AssignmentName = r.Assignment?.Title ?? "Unknown Assignment"
            }).ToList();
        }

        public async Task CreateReminderAsync(ReminderCreateViewModel model, Guid userId)
        {
            if (model.AssignmentId == Guid.Empty)
            {
                _logger.LogError("Invalid AssignmentId provided for Reminder creation.");
                throw new ArgumentException("AssignmentId cannot be empty.");
            }

            var reminder = new Reminder
            {
                Id = Guid.NewGuid(),
                AssignmentId = model.AssignmentId,
                Message = model.Message,
                ReminderDate = model.ReminderDate,
                UserId = userId
            };

            await _reminderRepository.AddAsync(reminder);
            await _activityLogService.LogActionAsync(userId, ActionType.Created, $"Created reminder with message '{reminder.Message}'", reminder.Id, "Reminder");

        }

        public async Task<ReminderViewModel?> GetReminderByIdAsync(Guid id, Guid userId)
        {
            var reminder = await _reminderRepository
                .GetQueryableWhere(r => r.Id == id && r.UserId == userId)
                .FirstOrDefaultAsync();

            if (reminder == null)
            {
                _logger.LogError($"Reminder with Id {id} not found for UserId {userId}.");
                return null;
            }

            return new ReminderViewModel
            {
                Id = reminder.Id,
                AssignmentId = reminder.AssignmentId,
                Message = reminder.Message,
                ReminderDate = reminder.ReminderDate
            };
        }

        public async Task UpdateReminderAsync(ReminderEditViewModel model, Guid userId)
        {
            var reminder = await _reminderRepository.GetByIdAsync(model.Id);
            if (reminder == null || reminder.UserId != userId)
            {
                throw new UnauthorizedAccessException("You cannot edit this reminder.");
            }

            reminder.Message = model.Message;
            reminder.ReminderDate = model.ReminderDate;

            await _reminderRepository.UpdateAsync(reminder);

            await _activityLogService.LogActionAsync(userId, ActionType.Updated, $"Updated reminder with message '{reminder.Message}'", reminder.Id, "Reminder");

        }

        public async Task<Guid?> DeleteReminderAsync(Guid id, Guid userId)
        {
            var reminder = await _reminderRepository
                .GetQueryableWhere(r => r.Id == id && r.UserId == userId)
                .Include(r => r.Assignment)
                .FirstOrDefaultAsync();

            if (reminder == null)
            {
                return null;
            }

            var assignmentId = reminder.AssignmentId;
            reminder.IsDeleted = true;
            await _reminderRepository.UpdateAsync(reminder);

            
            var assignmentTitle = reminder.Assignment?.Title ?? "Unknown Assignment";
            await _activityLogService.LogActionAsync(userId, ActionType.Deleted,
                $"Deleted reminder with message '{reminder.Message}' (linked to assignment: '{assignmentTitle}')",
                reminder.Id, "Reminder");

            return assignmentId;
        }

    }
}
