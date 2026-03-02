namespace ClinicaSim.Application.Interfaces;

public interface IPdfReportService
{
    Task<byte[]> GenerateSessionPdfAsync(string sessionCode, CancellationToken cancellationToken = default);
}
