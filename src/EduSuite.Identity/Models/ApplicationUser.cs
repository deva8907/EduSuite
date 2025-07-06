using Microsoft.AspNetCore.Identity;

namespace EduSuite.Identity.Models;

public class ApplicationUser : IdentityUser
{
    public string? TenantId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
} 