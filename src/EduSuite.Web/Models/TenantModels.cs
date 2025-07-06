using System.ComponentModel.DataAnnotations;

namespace EduSuite.Web.Models;

public class TenantDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public TenantSettingsDto? Settings { get; set; }
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
    public StorageSettingsDto? Storage { get; set; }
}

public class StorageSettingsDto
{
    public string Provider { get; set; } = "AzureBlob";
    public string ContainerName { get; set; } = string.Empty;
    public long MaxStorageInMB { get; set; } = 5120;
    public string[] AllowedFileTypes { get; set; } = Array.Empty<string>();
}

public class CreateTenantRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    [RegularExpression("^[a-zA-Z0-9-_]+$", ErrorMessage = "Code can only contain letters, numbers, hyphens, and underscores")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    public TenantSettingsRequest? Settings { get; set; }
}

public class UpdateTenantRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public TenantSettingsRequest? Settings { get; set; }
}

public class TenantSettingsRequest
{
    [Required]
    [RegularExpression(@"^(UTC|[A-Za-z]+/[A-Za-z_]+)$", ErrorMessage = "Invalid timezone format")]
    public string TimeZone { get; set; } = "UTC";

    [Required]
    [RegularExpression(@"^[a-z]{2}-[A-Z]{2}$", ErrorMessage = "Invalid locale format (e.g., en-US)")]
    public string Locale { get; set; } = "en-US";

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string CurrencyCode { get; set; } = "INR";

    [Required]
    public string DateFormat { get; set; } = "dd/MM/yyyy";

    [Required]
    public string TimeFormat { get; set; } = "HH:mm:ss";

    public bool UsesDaylightSaving { get; set; }

    [Range(1, 1000)]
    public int MaxUsersAllowed { get; set; } = 100;

    [Range(1, 10000)]
    public int MaxStudentsAllowed { get; set; } = 1000;

    public StorageSettingsRequest? Storage { get; set; }
}

public class StorageSettingsRequest
{
    [RegularExpression("^(AzureBlob|LocalFileSystem|S3)$", ErrorMessage = "Invalid storage provider")]
    public string Provider { get; set; } = "AzureBlob";

    [StringLength(63, MinimumLength = 3)]
    [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Container name can only contain lowercase letters, numbers, and hyphens")]
    public string ContainerName { get; set; } = string.Empty;

    [Range(1024, 10240)]
    public long MaxStorageInMB { get; set; } = 5120;

    public string[] AllowedFileTypes { get; set; } = Array.Empty<string>();
} 