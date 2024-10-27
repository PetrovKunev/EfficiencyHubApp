using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EfficiencyHub.Data.Models;
using Efficiency.Data.Models;

namespace EfficiencyHub.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Reminder> Reminders { get; set; } = null!;
        public virtual DbSet<Assignment> Tasks { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<ActivityLog> ActivityLogs { get; set; } = null!;
        public virtual DbSet<PerformanceReport> PerformanceReports { get; set; } = null!;
    }
}
