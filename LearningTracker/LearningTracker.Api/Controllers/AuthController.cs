using CSharpFunctionalExtensions;
using LearningTracker.Api.Controllers._base;
using LearningTracker.Api.Services.Base;
using LearningTracker.Application.UseCases.Auth.Login;
using LearningTracker.Application.UseCases.Auth.Register;
using LearningTracker.Contracts.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LearningTracker.Api.Controllers;

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

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new LoginCommand(request.Login, request.Password), ct);
        if (result.IsSuccess)
        {
            var accessToken = await _tokenProvider.GenerateAccessToken(result.Value, ct);
            return HandleResult(Result.Success(accessToken));
        }
        return HandleResult(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var result = await _mediator.Send(new RegisterUserCommand(request.Login, request.Password));
        return HandleResult(result);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return HandleResult(Result.Success());
    }
}
