namespace ClinicaSim.Application.Models;

public sealed record DifferentialModel(int Rank, string Text);
public sealed record FinalizeResult(string SessionCode, string Status, DateTimeOffset FinishedAt);
