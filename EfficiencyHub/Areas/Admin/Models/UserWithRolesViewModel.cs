using EfficiencyHub.Data.Models;

namespace EfficiencyHub.Web.Areas.Admin.Models
{
    public class UserWithRolesViewModel
    {
        public required ApplicationUser User { get; set; }
        public string Roles { get; set; } = string.Empty;
    }
}
