using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EduSuite.Database.Tests;

public class DatabaseFixture : IDisposable
{
    public EduSuiteDbContext Context { get; }
    public Guid TestTenantId { get; }
    public string TestUserId { get; }

    public DatabaseFixture()
    {
        TestTenantId = Guid.NewGuid();
        TestUserId = "test-user";

        var services = new ServiceCollection();
        
        var options = new DbContextOptionsBuilder<EduSuiteDbContext>()
            .UseInMemoryDatabase("EduSuiteTestDb")
            .Options;

        Context = new EduSuiteDbContext(
            options,
            new TestTenantProvider(TestTenantId),
            new TestUserProvider(TestUserId));
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}

public class TestTenantProvider : ITenantProvider
{
    private readonly Guid _tenantId;

    public TestTenantProvider(Guid tenantId)
    {
        _tenantId = tenantId;
    }

    public Guid GetCurrentTenantId() => _tenantId;
}

public class TestUserProvider : ICurrentUserProvider
{
    private readonly string _userId;

    public TestUserProvider(string userId)
    {
        _userId = userId;
    }

    public string GetCurrentUserId() => _userId;
} 