using CSharpFunctionalExtensions;
using LearningTracker.Domain.Entities;
using LearningTracker.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningTracker.Application.UseCases.Auth.Register;

internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result> 
{
    private readonly IUsersRepository _users;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterUserCommandHandler(IUsersRepository users, IPasswordHasher<User> passwordHasher) 
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken ct) 
    {
        var encryptedPassword = _passwordHasher.HashPassword(null!, request.Password);
        var user = new User(request.Login, encryptedPassword);
        await _users.Add(user, ct);
        return Result.Success();
    }
}