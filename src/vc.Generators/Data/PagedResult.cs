namespace VisionaryCoder.Generators.Data;

/// <summary>
/// Represents a page of results returned by paginated repository queries.
/// This type is emitted into consuming assemblies by the RepositoryGenerator.
/// </summary>
public sealed class PagedResult<T>(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
{
    public IReadOnlyList<T> Items { get; } = items ?? throw new ArgumentNullException(paramName: nameof(items));
    public int Page { get; } = page;
    public int PageSize { get; } = pageSize;
    public int TotalCount { get; } = totalCount;
    public int TotalPages => (int)Math.Ceiling(a: (double)TotalCount / PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}