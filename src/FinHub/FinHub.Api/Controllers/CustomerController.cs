using FinHub.Application.Modules.Customer.DTOs;
using FinHub.Application.Modules.Customer.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinHub.Api.Controllers;

[ApiController]
[Route("api/v1/customer")]
public sealed class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet("{customerId:guid}/profile")]
    public async Task<ActionResult<CustomerProfileDto>> GetProfile(Guid customerId, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await _customerService.GetProfileAsync(customerId, cancellationToken);
            return Ok(profile);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPut("{customerId:guid}/address")]
    public async Task<ActionResult<CustomerProfileDto>> UpdateAddress(
        Guid customerId,
        [FromBody] UpdateCustomerAddressCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var profile = await _customerService.UpdateAddressAsync(customerId, command, cancellationToken);
            return Ok(profile);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{customerId:guid}/national-id")]
    public async Task<ActionResult<CustomerProfileDto>> SetNationalId(
        Guid customerId,
        [FromBody] string nationalId,
        CancellationToken cancellationToken)
    {
        try
        {
            var profile = await _customerService.SetNationalIdAsync(customerId, nationalId, cancellationToken);
            return Ok(profile);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
