using CSharpFunctionalExtensions;
using LearningTracker.Api.Controllers._base;
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

    public AuthController(IMediator mediator) {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(new LoginCommand(request.Login, request.Password));
        if (result.IsSuccess)
        {
            var accessToken = _
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
