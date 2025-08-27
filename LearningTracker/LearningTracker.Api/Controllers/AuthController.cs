using CSharpFunctionalExtensions;
using LearningTracker.Api.Controllers._base;
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
    public IActionResult Login()
    {
        return HandleResult(Result.Success());
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
