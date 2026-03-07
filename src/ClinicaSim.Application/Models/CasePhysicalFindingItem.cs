namespace ClinicaSim.Application.Models;

public sealed record CasePhysicalFindingItem(Guid Id, Guid FindingId, string FindingName, string System, bool Present, string? DetailText);
