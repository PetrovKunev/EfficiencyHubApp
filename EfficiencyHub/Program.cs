using EfficiencyHub.Data;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Services.Data;
using EfficiencyHub.Web.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyHub.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Database and Identity Configuration
            string connectionString = builder.Configuration.GetConnectionString("SQLServer") ?? throw new InvalidOperationException("Connection string 'SQLServer' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Identity Configuration
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Hosted Service for Data Seeding
            builder.Services.AddHostedService<DatabaseSeederHostedService>();

            // Cookie Configuration
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.Redirect("/Home/LandingPage");
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.Redirect(options.AccessDeniedPath);
                    return Task.CompletedTask;
                };
            });

            // Scoped Services
            builder.Services.AddScoped<IRepository<Project>, ProjectRepository>();
            builder.Services.AddScoped<IRepository<Assignment>, AssignmentRepository>();
            builder.Services.AddScoped<IRepository<ActivityLog>, ActivityLogRepository>();
            builder.Services.AddScoped<IRepository<Reminder>, ReminderRepository>();
            builder.Services.AddScoped<IRepository<ProjectAssignment>, ProjectAssignmentRepository>();
            builder.Services.AddScoped<DashboardService>();
            builder.Services.AddScoped<ProjectService>();
            builder.Services.AddScoped<AssignmentService>();
            builder.Services.AddScoped<ActivityLogService>();
            builder.Services.AddScoped<ReminderService>();
            builder.Services.AddScoped<PerformanceReportService>();

            // MVC and Razor Pages Configuration
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Seed Roles and Admin User
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                await RoleSeeder.SeedRolesAndAdminAsync(roleManager, userManager, configuration);
            }

            // Middleware Configuration
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Handle500");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/Home/HandleError/{0}");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Routing
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=AdminDashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "dashboard",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=LandingPage}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}
