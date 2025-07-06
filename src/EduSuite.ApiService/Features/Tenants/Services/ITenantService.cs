using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EduSuite.ApiService.Features.Tenants.Models;

namespace EduSuite.ApiService.Features.Tenants.Services;

public interface ITenantService
{
    Task<IEnumerable<TenantDto>> GetAllTenantsAsync(CancellationToken cancellationToken = default);
    Task<TenantDto?> GetTenantByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TenantDto?> GetTenantByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<TenantDto> CreateTenantAsync(CreateTenantRequest request, CancellationToken cancellationToken = default);
    Task<TenantDto> UpdateTenantAsync(Guid id, UpdateTenantRequest request, CancellationToken cancellationToken = default);
    Task DeleteTenantAsync(Guid id, CancellationToken cancellationToken = default);
} 
