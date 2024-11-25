
namespace EfficiencyHub.Web.ViewModels
{
    public class ActivityLogViewModel
    {
        public DateTime Timestamp { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

    }
}
