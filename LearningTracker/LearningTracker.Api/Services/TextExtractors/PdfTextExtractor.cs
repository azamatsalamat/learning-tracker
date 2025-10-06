using System.Text;
using iText.Kernel.Pdf;
using LearningTracker.Infrastructure;
using LearningTracker.Services.Base;

namespace LearningTracker.Services.TextExtractors;

public class PdfTextExtractor : ITextExtractor
{
    private readonly ILogger<PdfTextExtractor> _logger;

    public PdfTextExtractor(ILogger<PdfTextExtractor> logger)
    {
        _logger = logger;
    }

    public Task<string> ExtractTextAsync(Stream fileStream, CancellationToken ct)
    {
        return Task.Run(() =>
        {
            _logger.LogInformation("Extracting text from PDF");
            var extractedText = new StringBuilder();

            try
            {
                using var pdfReader = new PdfReader(fileStream);
                using var pdfDocument = new PdfDocument(pdfReader);

                for (int pageNumber = 1; pageNumber <= pdfDocument.GetNumberOfPages(); pageNumber++)
                {
                    ct.ThrowIfCancellationRequested();

                    var page = pdfDocument.GetPage(pageNumber);
                    var pageText = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(page);
                    extractedText.AppendLine(pageText);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract text from PDF");
                throw new InvalidOperationException("Failed to extract text from PDF", ex);
            }

            return extractedText.ToString();
        }, ct);
    }

    public bool SupportsContentType(string contentType)
    {
        return contentType.Equals(MimeTypes.Pdf);
    }
}
