using EfficiencyHub.Data;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data.Repository;
using EfficiencyHub.Data.Repository.Interfaces;
using EfficiencyHub.Services.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EfficiencyHub.Web.Infrastructure.Data;


namespace EfficiencyHub.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            string connectionString = builder.Configuration.GetConnectionString("SQLServer") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

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

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Handle500");
                app.UseHsts();
            }

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                await RoleSeeder.EnsureRolesAsync(roleManager);
                await RoleSeeder.EnsureAdminUserAsync(userManager);
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/Home/HandleError/{0}");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

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
