namespace EduSuite.Database.Entities.Tenant;

public class StorageSettings
{
    public string Provider { get; set; } = "AzureBlob";
    public string ContainerName { get; set; } = string.Empty;
    public long MaxStorageInMB { get; set; } = 5120; // 5GB default
    public string[] AllowedFileTypes { get; set; } = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png" };
} 