using LearningTracker.Features.Profiles.Entities;
using LearningTracker.Services;
using LearningTracker.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LearningTracker.Features.Profiles;

[ApiController]
[Route("api/profile")]
public class ProfileController : LearningTrackerControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get profile by id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Profile), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProfile.Query(id), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Parse resume to profile
    /// </summary>
    [HttpPost("parse-resume")]
    [ProducesResponseType(typeof(Profile), StatusCodes.Status200OK)]
    public async Task<IActionResult> ParseResume(IFormFile file, [FromServices] TextExtractorService textExtractor, CancellationToken ct)
    {
        var text = await textExtractor.ExtractTextAsync(file, ct);
        var result = await _mediator.Send(new ParseResume.Command(text), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Update profile
    /// </summary>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update([FromBody] UpdateProfile.Command request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);
        return HandleResult(result);
    }
}
