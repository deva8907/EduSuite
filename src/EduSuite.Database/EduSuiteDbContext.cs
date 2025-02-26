using EduSuite.Database.Entities;
using EduSuite.Database.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EduSuite.Database;

public class EduSuiteDbContext : DbContext
{
    private readonly Guid _currentTenantId;
    private readonly string _currentUserId;
    private readonly ITenantProvider _tenantProvider;
    private readonly ICurrentUserProvider _currentUserProvider;

    public DbSet<Tenant> Tenants { get; set; } = null!;

    public EduSuiteDbContext(
        DbContextOptions<EduSuiteDbContext> options,
        ITenantProvider tenantProvider,
        ICurrentUserProvider currentUserProvider)
        : base(options)
    {
        _currentTenantId = tenantProvider.GetCurrentTenantId();
        _currentUserId = currentUserProvider.GetCurrentUserId();
        _tenantProvider = tenantProvider;
        _currentUserProvider = currentUserProvider;
    }

    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure global query filter for tenant isolation
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var tenantProperty = Expression.Property(parameter, "TenantId");
                var tenantValue = Expression.Constant(_currentTenantId);
                var isDeletedProperty = Expression.Property(parameter, "IsDeleted");
                var isFalse = Expression.Constant(false);

                var combinedFilter = Expression.AndAlso(
                    Expression.Equal(tenantProperty, tenantValue),
                    Expression.Equal(isDeletedProperty, isFalse));

                var lambda = Expression.Lambda(combinedFilter, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("Tenants");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Code).IsUnique();

            // Configure TenantSettings as owned entity
            entity.OwnsOne(e => e.Settings, settings =>
            {
                settings.Property(s => s.TimeZone).IsRequired();
                settings.Property(s => s.Locale).IsRequired();
                settings.Property(s => s.CurrencyCode).IsRequired();
                settings.Property(s => s.DateFormat).IsRequired();
                settings.Property(s => s.TimeFormat).IsRequired();

                // Configure StorageSettings as owned entity
                settings.OwnsOne(s => s.Storage, storage =>
                {
                    storage.Property(s => s.Provider).IsRequired();
                    storage.Property(s => s.ContainerName).IsRequired();
                    storage.Property(s => s.MaxStorageInMB).IsRequired();
                    storage.Property(s => s.AllowedFileTypes).HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
                });
            });
        });
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserId;
                    entry.Entity.TenantId = _currentTenantId;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = _currentUserId;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.DeletedBy = _currentUserId;
                    break;
            }
        }
    }
}
