using System;

namespace EduSuite.Database;

public interface ITenantProvider
{
    Guid GetCurrentTenantId();
} 