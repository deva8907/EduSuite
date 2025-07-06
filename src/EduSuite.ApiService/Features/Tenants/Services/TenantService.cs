// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

using EduSuite.ApiService.Features.Tenants.Models;
using EduSuite.Database.Entities.Tenant;
using EduSuite.Database.Repositories;

namespace EduSuite.ApiService.Features.Tenants.Services;

public class TenantService : ITenantService
{
    private readonly IRepository<Tenant> _tenantRepository;

    public TenantService(IRepository<Tenant> tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync(CancellationToken cancellationToken = default)
    {
        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);
        return tenants.Select(MapToDto);
    }

    public async Task<TenantDto?> GetTenantByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id, cancellationToken);
        return tenant != null ? MapToDto(tenant) : null;
    }

    public async Task<TenantDto?> GetTenantByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var tenant = (await _tenantRepository.FindAsync(t => t.Code == code, cancellationToken)).FirstOrDefault();
        return tenant != null ? MapToDto(tenant) : null;
    }

    public async Task<TenantDto> CreateTenantAsync(CreateTenantRequest request, CancellationToken cancellationToken = default)
    {
        // Check if tenant code already exists
        if ((await _tenantRepository.FindAsync(t => t.Code == request.Code, cancellationToken)).Any())
        {
            throw new InvalidOperationException($"Tenant with code {request.Code} already exists");
        }

        var tenant = new Tenant
        {
            Code = request.Code,
            Name = request.Name,
            IsActive = true,
            Settings = MapToSettings(request.Settings)
        };

        var created = await _tenantRepository.AddAsync(tenant, cancellationToken);
        return MapToDto(created);
    }

    public async Task<TenantDto> UpdateTenantAsync(Guid id, UpdateTenantRequest request, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id, cancellationToken);
        if (tenant == null)
        {
            throw new InvalidOperationException($"Tenant with ID {id} not found");
        }

        tenant.Name = request.Name;
        tenant.IsActive = request.IsActive;

        if (request.Settings != null)
        {
            tenant.Settings = MapToSettings(request.Settings);
        }

        var updated = await _tenantRepository.UpdateAsync(tenant, cancellationToken);
        return MapToDto(updated);
    }

    public async Task DeleteTenantAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _tenantRepository.DeleteAsync(id, cancellationToken);
    }

    private static TenantDto MapToDto(Tenant tenant)
    {
        return new TenantDto
        {
            Id = tenant.Id,
            Code = tenant.Code,
            Name = tenant.Name,
            IsActive = tenant.IsActive,
            Settings = new TenantSettingsDto
            {
                TimeZone = tenant.Settings.TimeZone,
                Locale = tenant.Settings.Locale,
                CurrencyCode = tenant.Settings.CurrencyCode,
                DateFormat = tenant.Settings.DateFormat,
                TimeFormat = tenant.Settings.TimeFormat,
                UsesDaylightSaving = tenant.Settings.UsesDaylightSaving,
                MaxUsersAllowed = tenant.Settings.MaxUsersAllowed,
                MaxStudentsAllowed = tenant.Settings.MaxStudentsAllowed,
                Storage = new StorageSettingsDto
                {
                    Provider = tenant.Settings.Storage.Provider,
                    ContainerName = tenant.Settings.Storage.ContainerName,
                    MaxStorageInMB = tenant.Settings.Storage.MaxStorageInMB,
                    AllowedFileTypes = tenant.Settings.Storage.AllowedFileTypes
                }
            }
        };
    }

    private static TenantSettings MapToSettings(TenantSettingsRequest? request)
    {
        if (request == null)
        {
            return new TenantSettings();
        }

        return new TenantSettings
        {
            TimeZone = request.TimeZone,
            Locale = request.Locale,
            CurrencyCode = request.CurrencyCode,
            DateFormat = request.DateFormat,
            TimeFormat = request.TimeFormat,
            UsesDaylightSaving = request.UsesDaylightSaving,
            MaxUsersAllowed = request.MaxUsersAllowed,
            MaxStudentsAllowed = request.MaxStudentsAllowed,
            Storage = request.Storage != null ? new StorageSettings
            {
                Provider = request.Storage.Provider,
                ContainerName = !string.IsNullOrEmpty(request.Storage.ContainerName) 
                    ? request.Storage.ContainerName 
                    : $"tenant-{Guid.NewGuid():N}",
                MaxStorageInMB = request.Storage.MaxStorageInMB,
                AllowedFileTypes = request.Storage.AllowedFileTypes?.Length > 0 
                    ? request.Storage.AllowedFileTypes 
                    : new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png" }
            } : new StorageSettings()
        };
    }
}
