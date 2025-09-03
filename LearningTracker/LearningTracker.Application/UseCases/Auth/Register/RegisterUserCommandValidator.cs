using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using FluentValidation;
using LearningTracker.Application.Configuration.Pipelines;
using LearningTracker.Domain.Repositories;

namespace LearningTracker.Application.UseCases.Auth.Register;

internal class RegisterUserCommandValidator : ValidationBehavior<RegisterUserCommand> {
    private readonly IUsersRepository _users;
    
    public RegisterUserCommandValidator(IUsersRepository users) {
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(255)
            .Matches(new Regex(@"^[a-zA-Z0-9!@#\$%\^&*\.\(\)]*$"))
            .WithMessage("Password should contain latin alphabet letters (a–z и A–Z), digits (0–9) and special characters !@#$%^&*.()");
        _users = users;
    }

    protected override async Task<Result> RequestValidateAsync(RegisterUserCommand command, CancellationToken ct) {
        var user = await _users.GetByLogin(command.Login, ct);
        if (user != null) {
            return Result.Failure("User with this login already exists");
        }
        
        return await base.RequestValidateAsync(command, ct);
    }
}