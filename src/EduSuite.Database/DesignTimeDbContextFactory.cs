using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EduSuite.Database;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EduSuiteDbContext>
{
    public EduSuiteDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EduSuiteDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EduSuite;Trusted_Connection=True;MultipleActiveResultSets=true");

        return new EduSuiteDbContext(
            optionsBuilder.Options,
            new DesignTimeTenantProvider(),
            new DesignTimeUserProvider());
    }
}

public class DesignTimeTenantProvider : ITenantProvider
{
    public Guid GetCurrentTenantId() => Guid.Empty;
}

public class DesignTimeUserProvider : ICurrentUserProvider
{
    public string GetCurrentUserId() => "SYSTEM";
} 