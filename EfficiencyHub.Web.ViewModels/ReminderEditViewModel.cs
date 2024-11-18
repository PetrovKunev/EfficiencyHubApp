namespace EfficiencyHub.Web.ViewModels
{
    public class ReminderEditViewModel
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ReminderDate { get; set; }
        public Guid AssignmentId { get; set; }
        public string AssignmentTitle { get; set; } = string.Empty;
    }
}
