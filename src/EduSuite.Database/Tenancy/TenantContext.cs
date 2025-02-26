using System;
using System.Threading;
using System.Threading.Tasks;
using EduSuite.Database.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EduSuite.Database.Tenancy;

public class TenantContext : ITenantContext
{
    private readonly EduSuiteDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private const string CacheKeyPrefix = "tenant_";
    private const int CacheTimeInMinutes = 30;

    public TenantContext(EduSuiteDbContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public Guid TenantId { get; private set; }
    public string TenantCode { get; private set; } = string.Empty;
    public Tenant CurrentTenant { get; private set; } = null!;
    public TenantSettings Settings { get; private set; } = null!;
    public bool IsInitialized { get; private set; }

    public async Task InitializeAsync(string tenantCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(tenantCode))
            throw new ArgumentException("Tenant code cannot be empty", nameof(tenantCode));

        var cacheKey = $"{CacheKeyPrefix}{tenantCode}";

        if (!_cache.TryGetValue(cacheKey, out Tenant? tenant))
        {
            tenant = await _dbContext.Set<Tenant>()
                .FirstOrDefaultAsync(t => t.Code == tenantCode && !t.IsDeleted && t.IsActive, cancellationToken);

            if (tenant == null)
                throw new TenantNotFoundException($"Tenant with code {tenantCode} not found or is inactive");

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheTimeInMinutes));

            _cache.Set(cacheKey, tenant, cacheOptions);
        }

        TenantId = tenant!.Id;
        TenantCode = tenant.Code;
        CurrentTenant = tenant;
        Settings = tenant.Settings;
        IsInitialized = true;
    }

    public void Reset()
    {
        TenantId = Guid.Empty;
        TenantCode = string.Empty;
        CurrentTenant = null!;
        Settings = null!;
        IsInitialized = false;
    }
} 