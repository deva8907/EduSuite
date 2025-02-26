using System;
using EduSuite.Database.HealthChecks;
using EduSuite.Database.Repositories;
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

        // Register health checks
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("sql_database_check");

        return services;
    }
} 