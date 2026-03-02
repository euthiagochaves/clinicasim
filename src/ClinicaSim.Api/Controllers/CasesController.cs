using ClinicaSim.Api.Dtos;
using ClinicaSim.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaSim.Api.Controllers;

[ApiController]
[Route("api/cases")]
public class CasesController(ICasesService casesService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CaseListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CaseListItemDto>>> GetCases(CancellationToken cancellationToken)
    {
        var items = await casesService.GetCasesAsync(cancellationToken);
        return Ok(items.Select(x => new CaseListItemDto(x.CaseId, x.FullName, x.Age, x.Sex, x.ChiefComplaint, x.Triage)).ToList());
    }
}
