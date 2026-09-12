using FinHub.Application.Modules.Budgeting.DTOs;
using FinHub.Application.Modules.Budgeting.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinHub.Api.Controllers;

[ApiController]
[Route("api/v1/budgets")]
public sealed class BudgetController : ControllerBase
{
    private readonly IBudgetingService _budgetingService;

    public BudgetController(IBudgetingService budgetingService)
    {
        _budgetingService = budgetingService;
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<BudgetOverviewDto>> GetCustomerBudgetOverview(Guid customerId, CancellationToken cancellationToken)
    {
        try
        {
            var overview = await _budgetingService.GetCustomerBudgetOverviewAsync(customerId, cancellationToken);
            return Ok(overview);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
