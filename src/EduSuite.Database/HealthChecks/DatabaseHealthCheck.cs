// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EduSuite.Database.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly EduSuiteDbContext _context;

    public DatabaseHealthCheck(EduSuiteDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Try to execute a simple query
            await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            return HealthCheckResult.Healthy("Database is responding normally.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is not responding.", ex);
        }
    }
}
