using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EduSuite.Database;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EduSuiteDbContext>
{
    public EduSuiteDbContext CreateDbContext(string[] args)
    {
        // Build configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(GetApiServicePath())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

        // Get connection string from configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "DefaultConnection string not found in appsettings.json. " +
                "Please ensure the connection string is configured in the ApiService project.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<EduSuiteDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new EduSuiteDbContext(
            optionsBuilder.Options,
            new DesignTimeTenantProvider(),
            new DesignTimeUserProvider());
    }

    private static string GetApiServicePath()
    {
        // Get the current directory (Database project)
        var currentDirectory = Directory.GetCurrentDirectory();
        
        // Navigate to the ApiService project directory
        var apiServicePath = Path.Combine(currentDirectory, "..", "EduSuite.ApiService");
        
        // If we're running from solution root, adjust path
        if (!Directory.Exists(apiServicePath))
        {
            apiServicePath = Path.Combine(currentDirectory, "src", "EduSuite.ApiService");
        }
        
        // If still not found, try relative to Database project
        if (!Directory.Exists(apiServicePath))
        {
            apiServicePath = Path.Combine(currentDirectory, "..", "EduSuite.ApiService");
        }

        if (!Directory.Exists(apiServicePath))
        {
            throw new DirectoryNotFoundException(
                $"Could not find EduSuite.ApiService directory. Searched paths: {apiServicePath}");
        }

        return apiServicePath;
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