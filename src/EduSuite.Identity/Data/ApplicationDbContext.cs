using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.EntityFramework.Extensions;
using Duende.IdentityServer.EntityFramework.Interfaces;
using Duende.IdentityServer.EntityFramework.Options;
using EduSuite.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduSuite.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IPersistedGrantDbContext
{
    private readonly IConfiguration _configuration;
    private readonly string? _tenantId;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _configuration = configuration;
        _tenantId = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-ID"].FirstOrDefault();
    }

    public DbSet<PersistedGrant> PersistedGrants { get; set; }
    public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }
    public DbSet<Key> Keys { get; set; }
    public DbSet<ServerSideSession> ServerSideSessions { get; set; }
    public DbSet<PushedAuthorizationRequest> PushedAuthorizationRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigurePersistedGrantContext(_configuration.GetSection("OperationalStore").Get<OperationalStoreOptions>());

        // Configure global query filter for tenant
        builder.Entity<ApplicationUser>()
            .HasQueryFilter(u => _tenantId == null || u.TenantId == _tenantId);
    }

    public Task<int> SaveChangesAsync() => base.SaveChangesAsync();
} 