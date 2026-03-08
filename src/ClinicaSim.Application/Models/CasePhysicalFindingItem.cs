namespace ClinicaSim.Application.Models;

public sealed record CasePhysicalFindingItem(Guid? OverrideId, Guid FindingId, string FindingName, string System, bool Present, string? DetailText, bool IsInherited, bool IsCaseSpecific, bool IsHighlighted);
