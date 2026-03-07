namespace ClinicaSim.Application.Models;

public sealed record QuestionBankItem(Guid Id, string Text, string Section, string Category, string? Tags, bool Active);
