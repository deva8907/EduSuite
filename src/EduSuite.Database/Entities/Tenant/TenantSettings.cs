using System;

namespace EduSuite.Database.Entities.Tenant;

public class TenantSettings
{
    public string TimeZone { get; set; } = "UTC";
    public string Locale { get; set; } = "en-US";
    public string CurrencyCode { get; set; } = "INR";
    public string DateFormat { get; set; } = "dd/MM/yyyy";
    public string TimeFormat { get; set; } = "HH:mm:ss";
    public bool UsesDaylightSaving { get; set; }
    public int MaxUsersAllowed { get; set; } = 100;
    public int MaxStudentsAllowed { get; set; } = 1000;
    public StorageSettings Storage { get; set; } = new();
} 