namespace ClinicaSim.Application.Common;

public class AppException(string message, int statusCode, IReadOnlyCollection<string>? errors = null) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public IReadOnlyCollection<string> Errors { get; } = errors ?? [message];
}
