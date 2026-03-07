namespace ClinicaSim.Application.Models;

public sealed record FindingBankItem(Guid Id, string Name, string System, string? Tags, bool Active);
