using LearningTracker.Features.Users;
using LearningTracker.Services;
using LearningTracker.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LearningTracker.Features.Profiles;

[ApiController]
[Route("api/auth")]
public class ProfileController : LearningTrackerControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("parse-resume")]
    public async Task<IActionResult> ParseResume(IFormFile file, [FromServices] TextExtractorService textExtractor,
        CancellationToken ct)
    {
        var text = await textExtractor.ExtractTextAsync(file, ct);
        var result = await _mediator.Send(new ParseResume.Command(text), ct);
        return HandleResult(result);
    }
}
