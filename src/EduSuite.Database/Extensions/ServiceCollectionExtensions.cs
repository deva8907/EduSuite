using System;
using EduSuite.Database.HealthChecks;
using EduSuite.Database.Repositories;
using EduSuite.Database.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EduSuite.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEduSuiteDatabase(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<EduSuiteDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            }));

        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register tenant services
        services.AddMemoryCache();
        services.AddScoped<ITenantContext, TenantContext>();

        // Register health checks
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("sql_database_check");

        return services;
    }
} 