namespace VisionaryCoder.Ifx;

public sealed class HttpAdapter(HttpClient client) : IHttpAdapter
{
    private readonly HttpClient _client = client ?? throw new ArgumentNullException(paramName: nameof(client));

    public async Task<string> GetAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(argument: requestUri);
        var response = await _client.GetAsync(requestUri: requestUri, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
    }

    public async Task<string> PostAsync(string requestUri, string content, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(argument: requestUri);
        ArgumentNullException.ThrowIfNull(argument: content);
        using var httpContent = new StringContent(content: content);
        var response = await _client.PostAsync(requestUri: requestUri, content: httpContent, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
    }
}
