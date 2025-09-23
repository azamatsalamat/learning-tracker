namespace LearningTracker.Application.Services;

public interface ITextExtractor
{
    Task<string> ExtractTextAsync(Stream fileStream, CancellationToken ct);
    bool SupportsContentType(string contentType);
}
