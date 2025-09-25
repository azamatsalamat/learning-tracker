namespace LearningTracker.Application.Services.TextExtractors;

public interface ITextExtractor
{
    Task<string> ExtractTextAsync(Stream fileStream, CancellationToken ct);
    bool SupportsContentType(string contentType);
}
