using System;

namespace EduSuite.ApiService.Features.Tenants.Models;

public class TenantDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public TenantSettingsDto Settings { get; set; } = new();
}

public class TenantSettingsDto
{
    public string TimeZone { get; set; } = "UTC";
    public string Locale { get; set; } = "en-US";
    public string CurrencyCode { get; set; } = "INR";
    public string DateFormat { get; set; } = "dd/MM/yyyy";
    public string TimeFormat { get; set; } = "HH:mm:ss";
    public bool UsesDaylightSaving { get; set; }
    public int MaxUsersAllowed { get; set; } = 100;
    public int MaxStudentsAllowed { get; set; } = 1000;
    public StorageSettingsDto Storage { get; set; } = new();
}

public class StorageSettingsDto
{
    public string Provider { get; set; } = "AzureBlob";
    public string ContainerName { get; set; } = string.Empty;
    public long MaxStorageInMB { get; set; } = 5120;
    public string[] AllowedFileTypes { get; set; } = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png" };
} 