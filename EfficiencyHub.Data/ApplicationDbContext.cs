using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EfficiencyHub.Data.Models;
using Microsoft.AspNetCore.Identity;
using EfficiencyHub.Web.Infrastructure.Data;

namespace EfficiencyHub.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
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
        public virtual DbSet<ProjectAssignment> ProjectAssignments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            DatabaseSeeder.SeedData(modelBuilder);
        }
    }

}
