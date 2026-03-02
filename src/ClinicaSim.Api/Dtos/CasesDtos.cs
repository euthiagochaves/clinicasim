namespace ClinicaSim.Api.Dtos;

public sealed record CaseListItemDto(Guid CaseId, string FullName, int Age, string Sex, string ChiefComplaint, string Triage);
