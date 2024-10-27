using Efficiency.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace EfficiencyHub.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            
            this.Id = Guid.NewGuid();
        }

        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
