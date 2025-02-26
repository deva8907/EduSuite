using System;
using System.Threading;
using System.Threading.Tasks;
using EduSuite.Database.Entities.Tenant;

namespace EduSuite.Database.Tenancy;

public interface ITenantContext
{
    Guid TenantId { get; }
    string TenantCode { get; }
    Tenant CurrentTenant { get; }
    TenantSettings Settings { get; }
    bool IsInitialized { get; }
    Task InitializeAsync(string tenantCode, CancellationToken cancellationToken = default);
    void Reset();
} 