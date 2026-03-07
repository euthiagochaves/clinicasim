using ClinicaSim.Api.Dtos;
using ClinicaSim.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaSim.Api.Controllers;

[ApiController]
[Route("api/questions")]
public class QuestionsController(IQuestionBankService questionBankService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<QuestionBankItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<QuestionBankItemDto>>> GetQuestions([FromQuery] string? section, CancellationToken cancellationToken)
    {
        var items = await questionBankService.GetQuestionsAsync(section, cancellationToken);
        return Ok(items.Select(x => new QuestionBankItemDto(x.Id, x.Text, x.Section, x.Category, x.Tags, x.Active)).ToList());
    }
}
