using System;

namespace EduSuite.Database.Tenancy;

public class TenantNotFoundException : Exception
{
    public TenantNotFoundException(string message) : base(message)
    {
    }

    public TenantNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
} 