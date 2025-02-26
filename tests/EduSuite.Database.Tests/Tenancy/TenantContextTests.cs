using System;
using System.Threading.Tasks;
using EduSuite.Database.Entities.Tenant;
using EduSuite.Database.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace EduSuite.Database.Tests.Tenancy;

public class TenantContextTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly IMemoryCache _cache;
    private readonly ITenantContext _tenantContext;

    public TenantContextTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _cache = new MemoryCache(new MemoryCacheOptions());
        _tenantContext = new TenantContext(_fixture.Context, _cache);
    }

    [Fact]
    public async Task InitializeAsync_WithValidTenantCode_ShouldInitializeTenantContext()
    {
        // Arrange
        var tenant = new Tenant
        {
            Code = "TEST001",
            Name = "Test Tenant",
            IsActive = true,
            Settings = new TenantSettings
            {
                TimeZone = "Asia/Kolkata",
                Locale = "en-IN"
            }
        };

        await _fixture.Context.Set<Tenant>().AddAsync(tenant);
        await _fixture.Context.SaveChangesAsync();

        // Act
        await _tenantContext.InitializeAsync(tenant.Code);

        // Assert
        Assert.True(_tenantContext.IsInitialized);
        Assert.Equal(tenant.Id, _tenantContext.TenantId);
        Assert.Equal(tenant.Code, _tenantContext.TenantCode);
        Assert.Equal("Asia/Kolkata", _tenantContext.Settings.TimeZone);
    }

    [Fact]
    public async Task InitializeAsync_WithInvalidTenantCode_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<TenantNotFoundException>(
            () => _tenantContext.InitializeAsync("INVALID"));
    }

    [Fact]
    public async Task InitializeAsync_WithInactiveTenant_ShouldThrowException()
    {
        // Arrange
        var tenant = new Tenant
        {
            Code = "INACTIVE001",
            Name = "Inactive Tenant",
            IsActive = false
        };

        await _fixture.Context.Set<Tenant>().AddAsync(tenant);
        await _fixture.Context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TenantNotFoundException>(
            () => _tenantContext.InitializeAsync(tenant.Code));
    }

    [Fact]
    public async Task Reset_ShouldClearTenantContext()
    {
        // Arrange
        var tenant = new Tenant
        {
            Code = "TEST002",
            Name = "Test Tenant 2",
            IsActive = true
        };

        await _fixture.Context.Set<Tenant>().AddAsync(tenant);
        await _fixture.Context.SaveChangesAsync();
        await _tenantContext.InitializeAsync(tenant.Code);

        // Act
        _tenantContext.Reset();

        // Assert
        Assert.False(_tenantContext.IsInitialized);
        Assert.Equal(Guid.Empty, _tenantContext.TenantId);
        Assert.Empty(_tenantContext.TenantCode);
    }
} 