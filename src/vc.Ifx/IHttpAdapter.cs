namespace VisionaryCoder.Ifx;

public interface IHttpAdapter
{
    Task<string> GetAsync(string requestUri, CancellationToken cancellationToken = default);
    Task<string> PostAsync(string requestUri, string content, CancellationToken cancellationToken = default);
}
