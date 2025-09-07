using CSharpFunctionalExtensions;
using LearningTracker.Domain.Entities;
using LearningTracker.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningTracker.Application.UseCases.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<User>>
{
    private readonly IUsersRepository _users;
    private readonly IPasswordHasher<User> _passwordHasher;

    public LoginCommandHandler(IUsersRepository users, IPasswordHasher<User> passwordHasher)
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<User>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _users.GetByLogin(request.Login, ct);
        if (user == null) {
            return Result.Failure<User>(ErrorMessages.InvalidLoginOrPassword);
        }

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
        if (passwordVerificationResult == PasswordVerificationResult.Failed) {
            return Result.Failure<User>(ErrorMessages.InvalidLoginOrPassword);
        }

        return Result.Success(user);
    }
}
