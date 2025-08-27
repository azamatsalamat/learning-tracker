using CSharpFunctionalExtensions;
using FluentValidation;
using LearningTracker.Application.Configuration.Pipelines;
using LearningTracker.Domain.Repositories;

namespace LearningTracker.Application.UseCases.Auth.Register;

internal class RegisterUserCommandValidator : ValidationBehavior<RegisterUserCommand> {
    private readonly IUsersRepository _users;
    
    public RegisterUserCommandValidator(IUsersRepository users) {
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
        _users = users;
    }

    protected override async Task<Result> RequestValidateAsync(RegisterUserCommand command, CancellationToken ct) {
        var user = await _users.GetByLogin(command.Login, ct);
        if (user != null) {
            return Result.Failure("A user with the same login already exists");
        }
        
        return await base.RequestValidateAsync(command, ct);
    }
}