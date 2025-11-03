using CSharpFunctionalExtensions;
using LearningTracker.Services.Base;
using LearningTracker.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LearningTracker.Features.Users;

[ApiController]
[Route("api/auth")]
public class AuthController : LearningTrackerControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenProvider _tokenProvider;

    public AuthController(IMediator mediator, ITokenProvider tokenProvider)
    {
        _mediator = mediator;
        _tokenProvider = tokenProvider;
    }

    /// <summary>
    /// Register user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new Register.Command(request.Login, request.Password), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new Login.Command(request.Login, request.Password), ct);
        if (result.IsSuccess)
        {
            var accessToken = await _tokenProvider.GenerateAccessToken(result.Value, ct);
            return HandleResult(Result.Success(accessToken));
        }
        return HandleResult(result);
    }
}
