// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

using EduSuite.ApiService.Features.Tenants.Services;
using EduSuite.Database;
using EduSuite.Database.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EduSuite.Tests.Fixtures;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove any existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<EduSuiteDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register test providers first
            services.AddScoped<ITenantProvider, DesignTimeTenantProvider>();
            services.AddScoped<ICurrentUserProvider, DesignTimeUserProvider>();

            // Configure DbContext to use in-memory database
            services.AddDbContext<EduSuiteDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Add tenant service
            services.AddScoped<ITenantService, TenantService>();
        });
    }
}

public class TestTenantProvider : ITenantProvider
{
    public Guid GetCurrentTenantId() => Guid.Empty;
}

public class TestUserProvider : ICurrentUserProvider
{
    public string GetCurrentUserId() => "test-user";
}
