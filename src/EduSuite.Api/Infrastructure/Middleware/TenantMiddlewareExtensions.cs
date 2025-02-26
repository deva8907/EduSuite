using Microsoft.AspNetCore.Builder;

namespace EduSuite.Api.Infrastructure.Middleware;

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantMiddleware>();
    }
} 