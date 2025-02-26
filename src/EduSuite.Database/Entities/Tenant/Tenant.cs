using System;

namespace EduSuite.Database.Entities.Tenant;

public class Tenant : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public TenantSettings Settings { get; set; } = new();
} 