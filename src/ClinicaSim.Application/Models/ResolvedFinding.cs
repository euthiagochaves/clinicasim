namespace ClinicaSim.Application.Models;

public sealed record ResolvedFinding(Guid CaseId, Guid FindingId, bool Present, string? DetailText);
