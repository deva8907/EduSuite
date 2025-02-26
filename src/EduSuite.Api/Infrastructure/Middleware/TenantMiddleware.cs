using System;
using System.Threading.Tasks;
using EduSuite.Database.Tenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EduSuite.Api.Infrastructure.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;
    private const string TenantHeaderName = "X-Tenant-Code";

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        try
        {
            if (!context.Request.Headers.TryGetValue(TenantHeaderName, out var tenantCode))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = $"Missing required header: {TenantHeaderName}" });
                return;
            }

            await tenantContext.InitializeAsync(tenantCode!);
            await _next(context);
        }
        catch (TenantNotFoundException ex)
        {
            _logger.LogWarning(ex, "Tenant not found");
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing tenant context");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "An error occurred processing the tenant" });
        }
        finally
        {
            tenantContext.Reset();
        }
    }
} 