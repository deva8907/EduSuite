using System;
using EduSuite.Database.Tenancy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace EduSuite.Api.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequiresTenantAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var tenantContext = context.HttpContext.RequestServices.GetService<ITenantContext>();

        if (tenantContext == null || !tenantContext.IsInitialized)
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Tenant context is not initialized" });
            return;
        }

        base.OnActionExecuting(context);
    }
} 