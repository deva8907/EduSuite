// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

using EduSuite.ApiService.Features.Tenants.Models;
using EduSuite.ApiService.Features.Tenants.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace EduSuite.ApiService.Features.Tenants;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    /// <summary>
    /// Test endpoint to verify API is accessible.
    /// </summary>
    [HttpGet("test")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public ActionResult<string> Test()
    {
        return Ok("API is working");
    }

    /// <summary>
    /// Gets all tenants.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all tenants.</returns>
    /// <response code="200">Returns the list of tenants.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TenantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TenantDto>>> GetTenants(CancellationToken cancellationToken)
    {
        var tenants = await _tenantService.GetAllTenantsAsync(cancellationToken);
        return Ok(tenants);
    }

    /// <summary>
    /// Gets a tenant by ID.
    /// </summary>
    /// <param name="id">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested tenant.</returns>
    /// <response code="200">Returns the requested tenant.</response>
    /// <response code="404">If the tenant is not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TenantDto>> GetTenantById(Guid id, CancellationToken cancellationToken)
    {
        var tenant = await _tenantService.GetTenantByIdAsync(id, cancellationToken);
        if (tenant == null)
        {
            return NotFound();
        }

        return Ok(tenant);
    }

    /// <summary>
    /// Gets a tenant by code.
    /// </summary>
    /// <param name="code">The tenant code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested tenant.</returns>
    /// <response code="200">Returns the requested tenant.</response>
    /// <response code="404">If the tenant is not found.</response>
    [HttpGet("by-code/{code}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TenantDto>> GetTenantByCode(string code, CancellationToken cancellationToken)
    {
        var tenant = await _tenantService.GetTenantByCodeAsync(code, cancellationToken);
        if (tenant == null)
        {
            return NotFound();
        }

        return Ok(tenant);
    }

    /// <summary>
    /// Creates a new tenant.
    /// </summary>
    /// <param name="request">The tenant creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created tenant.</returns>
    /// <response code="201">Returns the newly created tenant.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="409">If a tenant with the same code already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TenantDto>> CreateTenant(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var tenant = await _tenantService.CreateTenantAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetTenantById), new { id = tenant.Id }, tenant);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing tenant.
    /// </summary>
    /// <param name="id">The tenant ID.</param>
    /// <param name="request">The tenant update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated tenant.</returns>
    /// <response code="200">Returns the updated tenant.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the tenant is not found.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TenantDto>> UpdateTenant(Guid id, UpdateTenantRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var tenant = await _tenantService.UpdateTenantAsync(id, request, cancellationToken);
            return Ok(tenant);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a tenant.
    /// </summary>
    /// <param name="id">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the tenant was successfully deleted.</response>
    /// <response code="404">If the tenant is not found.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTenant(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _tenantService.DeleteTenantAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}
