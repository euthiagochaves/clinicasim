namespace ClinicaSim.Application.Models;

public sealed record SessionInfoResult(
    string SessionCode,
    string Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? FinishedAt,
    CaseListItem Case);
