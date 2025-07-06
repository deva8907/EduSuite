using System.Net.Http.Json;
using System.Text.Json;
using EduSuite.Web.Models;

namespace EduSuite.Web.Services;

public class TenantApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public TenantApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IEnumerable<TenantDto>> GetTenantsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/tenants");
            response.EnsureSuccessStatusCode();
            
            var tenants = await response.Content.ReadFromJsonAsync<IEnumerable<TenantDto>>(_jsonOptions);
            return tenants ?? Enumerable.Empty<TenantDto>();
        }
        catch (HttpRequestException)
        {
            return Enumerable.Empty<TenantDto>();
        }
    }

    public async Task<TenantDto?> GetTenantByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/tenants/{id}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<TenantDto>(_jsonOptions);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<TenantDto?> GetTenantByCodeAsync(string code)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/tenants/by-code/{code}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<TenantDto>(_jsonOptions);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<TenantDto?> CreateTenantAsync(CreateTenantRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/tenants", request, _jsonOptions);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<TenantDto>(_jsonOptions);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<TenantDto?> UpdateTenantAsync(Guid id, UpdateTenantRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/tenants/{id}", request, _jsonOptions);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<TenantDto>(_jsonOptions);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<bool> DeleteTenantAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/tenants/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
} 