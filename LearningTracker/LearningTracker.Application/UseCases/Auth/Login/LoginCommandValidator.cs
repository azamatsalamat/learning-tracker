using FluentValidation;
using LearningTracker.Application.Configuration.Pipelines;
using LearningTracker.Domain.Entities;

namespace LearningTracker.Application.UseCases.Auth.Login;

internal class LoginCommandValidator : ValidationBehavior<LoginCommand, User>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
