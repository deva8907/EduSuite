// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

using EduSuite.ApiService.Features.Tenants.Models;
using EduSuite.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace EduSuite.Tests.Features.Tenants;

public class TenantsControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TenantsControllerTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTenant_WithValidData_ShouldCreateTenant()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CreateTenantRequest
        {
            Code = "TEST001",
            Name = "Test Tenant",
            Settings = new TenantSettingsRequest
            {
                TimeZone = "Asia/Kolkata",
                Locale = "en-IN",
                CurrencyCode = "INR"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/tenants", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var tenant = await response.Content.ReadFromJsonAsync<TenantDto>();
        Assert.NotNull(tenant);
        Assert.Equal(request.Code, tenant.Code);
        Assert.Equal(request.Name, tenant.Name);
        Assert.True(tenant.IsActive);
    }

    [Fact]
    public async Task CreateTenant_WithDuplicateCode_ShouldReturnConflict()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CreateTenantRequest
        {
            Code = "TEST002",
            Name = "Test Tenant"
        };

        // Act
        await client.PostAsJsonAsync("/api/tenants", request);
        var response = await client.PostAsJsonAsync("/api/tenants", request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetTenant_WithValidId_ShouldReturnTenant()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createRequest = new CreateTenantRequest
        {
            Code = "TEST003",
            Name = "Test Tenant"
        };

        var createResponse = await client.PostAsJsonAsync("/api/tenants", createRequest);
        var createdTenant = await createResponse.Content.ReadFromJsonAsync<TenantDto>();

        // Act
        var response = await client.GetAsync($"/api/tenants/{createdTenant!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tenant = await response.Content.ReadFromJsonAsync<TenantDto>();
        Assert.NotNull(tenant);
        Assert.Equal(createdTenant.Id, tenant.Id);
        Assert.Equal(createRequest.Code, tenant.Code);
    }

    [Fact]
    public async Task UpdateTenant_WithValidData_ShouldUpdateTenant()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createRequest = new CreateTenantRequest
        {
            Code = "TEST004",
            Name = "Test Tenant"
        };

        var createResponse = await client.PostAsJsonAsync("/api/tenants", createRequest);
        var createdTenant = await createResponse.Content.ReadFromJsonAsync<TenantDto>();

        var updateRequest = new UpdateTenantRequest
        {
            Name = "Updated Test Tenant",
            IsActive = true,
            Settings = new TenantSettingsRequest
            {
                TimeZone = "Europe/London",
                Locale = "en-US",
                CurrencyCode = "USD"
            }
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/tenants/{createdTenant!.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tenant = await response.Content.ReadFromJsonAsync<TenantDto>();
        Assert.NotNull(tenant);
        Assert.Equal(updateRequest.Name, tenant.Name);
        Assert.Equal(updateRequest.Settings.TimeZone, tenant.Settings.TimeZone);
    }

    [Fact]
    public async Task DeleteTenant_WithValidId_ShouldDeleteTenant()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createRequest = new CreateTenantRequest
        {
            Code = "TEST005",
            Name = "Test Tenant"
        };

        var createResponse = await client.PostAsJsonAsync("/api/tenants", createRequest);
        var createdTenant = await createResponse.Content.ReadFromJsonAsync<TenantDto>();

        // Act
        var deleteResponse = await client.DeleteAsync($"/api/tenants/{createdTenant!.Id}");
        var getResponse = await client.GetAsync($"/api/tenants/{createdTenant.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
