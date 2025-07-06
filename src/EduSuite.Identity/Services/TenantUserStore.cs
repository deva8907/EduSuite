using EduSuite.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EduSuite.Identity.Data;

namespace EduSuite.Identity.Services;

public class TenantUserStore : UserStore<ApplicationUser>
{
    private readonly string? _tenantId;

    public TenantUserStore(
        ApplicationDbContext context,
        IdentityErrorDescriber? describer = null,
        IHttpContextAccessor httpContextAccessor = null) 
        : base(context, describer)
    {
        _tenantId = httpContextAccessor?.HttpContext?.Request.Headers["X-Tenant-ID"].FirstOrDefault();
    }

    public override async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(user.TenantId))
        {
            user.TenantId = _tenantId;
        }

        return await base.CreateAsync(user, cancellationToken);
    }

    public override async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await base.FindByIdAsync(userId, cancellationToken);
        return user?.TenantId == _tenantId ? user : null;
    }

    public override async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        var user = await base.FindByNameAsync(normalizedUserName, cancellationToken);
        return user?.TenantId == _tenantId ? user : null;
    }

    public override async Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        var user = await base.FindByEmailAsync(normalizedEmail, cancellationToken);
        return user?.TenantId == _tenantId ? user : null;
    }

    public override IQueryable<ApplicationUser> Users
    {
        get
        {
            var users = base.Users;
            if (!string.IsNullOrEmpty(_tenantId))
            {
                users = users.Where(u => u.TenantId == _tenantId);
            }
            return users;
        }
    }
} 