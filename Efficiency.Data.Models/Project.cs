using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace Efficiency.Data.Models
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }  = null!;

        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        
        //??
        public string  UserId { get; set; }
        public IdentityUser User { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        
        //Soft delete
        public bool IsDeleted { get; set; }
    }
}
