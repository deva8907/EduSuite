using System.ComponentModel.DataAnnotations;

namespace EduSuite.ApiService.Features.Tenants.Models;

public class UpdateTenantRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public TenantSettingsRequest? Settings { get; set; }
} 