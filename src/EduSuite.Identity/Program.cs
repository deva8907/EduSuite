using EduSuite.Identity.Configuration;
using EduSuite.Identity.Data;
using EduSuite.Identity.Models;
using EduSuite.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduSuite.Identity;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.

        builder.Services.AddRazorPages();

        // Configure DbContext
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Configure Identity
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;

            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddUserStore<TenantUserStore>()
        .AddDefaultTokenProviders();

        // Configure IdentityServer
        builder.Services.AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;
        })
        .AddAspNetIdentity<ApplicationUser>()
        .AddConfigurationStore(options =>
        {
            options.ConfigureDbContext = b => 
                b.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        })
        .AddOperationalStore(options =>
        {
            options.ConfigureDbContext = b => 
                b.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            options.EnableTokenCleanup = true;
            options.TokenCleanupInterval = 3600;
        })
        .AddInMemoryIdentityResources(IdentityConfig.IdentityResources)
        .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
        .AddInMemoryClients(IdentityConfig.Clients)
        .AddDeveloperSigningCredential(); // TODO: Replace with proper certificate in production

        // Add authorization policies
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminRole", policy =>
                policy.RequireRole("Admin"));
            
            options.AddPolicy("RequireTeacherRole", policy =>
                policy.RequireRole("Teacher"));
            
            options.AddPolicy("RequireStaffRole", policy =>
                policy.RequireRole("Staff"));
        });

        // Add HttpContextAccessor
        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages();

        // Seed initial data if needed
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }

        app.Run();
    }
}
