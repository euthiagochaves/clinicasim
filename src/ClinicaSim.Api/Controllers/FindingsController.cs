using ClinicaSim.Api.Dtos;
using ClinicaSim.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaSim.Api.Controllers;

[ApiController]
[Route("api/findings")]
public class FindingsController(IFindingBankService findingBankService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<FindingBankItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<FindingBankItemDto>>> GetFindings([FromQuery] string? system, CancellationToken cancellationToken)
    {
        var items = await findingBankService.GetFindingsAsync(system, cancellationToken);
        return Ok(items.Select(x => new FindingBankItemDto(x.Id, x.Name, x.System, x.Tags, x.Active)).ToList());
    }
}
