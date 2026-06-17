namespace VisionaryCoder.Ifx;

public sealed class HttpAdapter : IHttpAdapter
{
    private readonly HttpClient _client;

    public HttpAdapter(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<string> GetAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestUri);
        var response = await _client.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<string> PostAsync(string requestUri, string content, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestUri);
        ArgumentNullException.ThrowIfNull(content);
        using var httpContent = new StringContent(content);
        var response = await _client.PostAsync(requestUri, httpContent, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
