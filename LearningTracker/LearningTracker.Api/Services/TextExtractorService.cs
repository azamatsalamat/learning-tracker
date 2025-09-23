using LearningTracker.Application.Services;

namespace LearningTracker.Api.Services;

public class TextExtractorService
{
    private readonly IEnumerable<ITextExtractor> _textExtractors;

    public TextExtractorService(IEnumerable<ITextExtractor> textExtractors)
    {
        _textExtractors = textExtractors;
    }

    public async Task<string> ExtractTextAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file provided or file is empty.");

        var contentType = file.ContentType;
        var extractor = _textExtractors.FirstOrDefault(e => e.SupportsContentType(contentType));

        if (extractor == null)
            throw new NotSupportedException($"File type '{contentType}' is not supported. Supported types: PDF, DOCX, TXT");

        using var stream = file.OpenReadStream();
        return await extractor.ExtractTextAsync(stream, cancellationToken);
    }

}
