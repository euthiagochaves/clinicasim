namespace ClinicaSim.Application.Models;

public sealed record CaseListItem(Guid CaseId, string FullName, int Age, string Sex, string ChiefComplaint, string Triage);
