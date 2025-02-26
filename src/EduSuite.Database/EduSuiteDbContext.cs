using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EduSuite.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduSuite.Database;

public class EduSuiteDbContext : DbContext
{
    private readonly Guid _currentTenantId;
    private readonly string _currentUserId;

    public EduSuiteDbContext(
        DbContextOptions<EduSuiteDbContext> options,
        ITenantProvider tenantProvider,
        ICurrentUserProvider currentUserProvider) 
        : base(options)
    {
        _currentTenantId = tenantProvider.GetCurrentTenantId();
        _currentUserId = currentUserProvider.GetCurrentUserId();
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