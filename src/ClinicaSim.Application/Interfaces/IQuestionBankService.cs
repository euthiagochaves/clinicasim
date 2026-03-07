using ClinicaSim.Application.Models;

namespace ClinicaSim.Application.Interfaces;

public interface IQuestionBankService
{
    Task<IReadOnlyCollection<QuestionBankItem>> GetQuestionsAsync(string? section, CancellationToken cancellationToken = default);
}
