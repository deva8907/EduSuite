// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

using Microsoft.EntityFrameworkCore;
using System;

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

        var options = new DbContextOptionsBuilder<EduSuiteDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var tenantProvider = new TestTenantProvider(TestTenantId);
        var userProvider = new TestUserProvider(TestUserId);

        Context = new EduSuiteDbContext(
            options,
            tenantProvider,
            userProvider);
        Context.Database.EnsureCreated();
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
