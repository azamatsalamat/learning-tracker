using FluentAssertions;
using LearningTracker.Application.UseCases.Auth.Register;
using LearningTracker.Domain.Entities;
using LearningTracker.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace LearningTracker.Application.Tests.Users;

public class RegisterUserCommandTests 
{
    private static readonly RegisterUserCommand Command = new("testUser", "Qwerty123!" );
    private readonly RegisterUserCommandHandler _handler;
    private readonly IUsersRepository _usersMock;
    private readonly IPasswordHasher<User> _passwordHasherMock;

    public RegisterUserCommandTests() 
    {
        _usersMock = Substitute.For<IUsersRepository>();
        _passwordHasherMock = Substitute.For<IPasswordHasher<User>>();
        _handler = new RegisterUserCommandHandler(_usersMock, _passwordHasherMock);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_User_Already_Exists()
    {
        _usersMock.GetByLogin(Command.Login, CancellationToken.None).Returns(new User(Command.Login, Command.Password));
        var result = await _handler.Handle(Command, CancellationToken.None);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User already exists");
    }
}