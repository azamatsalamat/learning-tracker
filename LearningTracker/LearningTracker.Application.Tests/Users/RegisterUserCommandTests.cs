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
    private readonly RegisterUserCommandValidator _validator;
    private readonly IUsersRepository _usersMock;
    private readonly IPasswordHasher<User> _passwordHasherMock;

    public RegisterUserCommandTests()
    {
        _usersMock = Substitute.For<IUsersRepository>();
        _passwordHasherMock = Substitute.For<IPasswordHasher<User>>();
        _handler = new RegisterUserCommandHandler(_usersMock, _passwordHasherMock);
        _validator = new RegisterUserCommandValidator(_usersMock);
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_User_Already_Exists()
    {
        _usersMock.GetByLogin(Command.Login, CancellationToken.None).Returns(new User(Command.Login, Command.Password));
        var validationResult = await _validator.RequestValidateAsync(Command, CancellationToken.None);
        validationResult.IsFailure.Should().BeTrue();
        validationResult.Error.Should().Be("User with this login already exists");
    }

    [Fact]
    public async Task FluentValidation_Should_Return_Error_When_Login_Is_Empty()
    {
        var command = new RegisterUserCommand("", "Qwerty123!");
        var validationResult = await _validator.ValidateAsync(command, CancellationToken.None);
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Login");
    }

    [Fact]
    public async Task FluentValidation_Should_Return_Error_When_Password_Is_Empty()
    {
        var command = new RegisterUserCommand("testUser", "");
        var validationResult = await _validator.ValidateAsync(command, CancellationToken.None);
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task FluentValidation_Should_Return_Error_When_Password_Is_Too_Short()
    {
        var command = new RegisterUserCommand("testUser", "Qw1!");
        var validationResult = await _validator.ValidateAsync(command, CancellationToken.None);
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task FluentValidation_Should_Return_Error_When_Password_Is_Too_Long()
    {
        var longPassword = new string('A', 256);
        var command = new RegisterUserCommand("testUser", longPassword);
        var validationResult = await _validator.ValidateAsync(command, CancellationToken.None);
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task FluentValidation_Should_Return_Error_When_Password_Contains_Invalid_Characters()
    {
        var command = new RegisterUserCommand("testUser", "Qwerty123!@#$%^&*()[]{}");
        var validationResult = await _validator.ValidateAsync(command, CancellationToken.None);
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Password" && 
            e.ErrorMessage.Contains("Password should contain latin alphabet letters"));
    }

    [Fact]
    public async Task FluentValidation_Should_Return_Success_When_Password_Contains_Valid_Characters()
    {
        var command = new RegisterUserCommand("testUser", "Qwerty123!@#$%^&*().");
        var validationResult = await _validator.ValidateAsync(command, CancellationToken.None);
        validationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Call_PasswordHasher_With_Correct_Parameters()
    {
        await _handler.Handle(Command, CancellationToken.None);
        _passwordHasherMock.Received(1).HashPassword(null!, Command.Password);
    }

    [Fact]
    public async Task Handle_Should_Call_UsersRepository_Add_With_Correct_User()
    {
        var hashedPassword = "hashedPassword123";
        _passwordHasherMock.HashPassword(null!, Command.Password).Returns(hashedPassword);
        
        await _handler.Handle(Command, CancellationToken.None);
        
        await _usersMock.Received(1).Add(Arg.Is<User>(u => 
            u.Login == Command.Login && u.Password == hashedPassword), 
            CancellationToken.None);
    }

    [Fact]
    public async Task RequestValidate_Should_Return_Success_When_User_Does_Not_Exist()
    {
        _usersMock.GetByLogin(Command.Login, CancellationToken.None).Returns((User?)null);
        var validationResult = await _validator.RequestValidateAsync(Command, CancellationToken.None);
        validationResult.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Complete_Validation_Should_Return_Success_When_All_Conditions_Met()
    {
        _usersMock.GetByLogin(Command.Login, CancellationToken.None).Returns((User?)null);
        
        var fluentValidationResult = await _validator.ValidateAsync(Command, CancellationToken.None);
        fluentValidationResult.IsValid.Should().BeTrue();
        
        var businessLogicResult = await _validator.RequestValidateAsync(Command, CancellationToken.None);
        businessLogicResult.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("ValidPass1!")]
    [InlineData("Test123@")]
    [InlineData("MyPass#456")]
    [InlineData("Secure$789")]
    [InlineData("Password%123")]
    [InlineData("User^456")]
    [InlineData("Login&789")]
    [InlineData("Test*123")]
    [InlineData("Pass(456)")]
    [InlineData("User.789")]
    public async Task FluentValidation_Should_Accept_Valid_Password_Formats(string password)
    {
        var command = new RegisterUserCommand("testUser", password);
        var validationResult = await _validator.ValidateAsync(command, CancellationToken.None);
        validationResult.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task FluentValidation_Should_Reject_Empty_Or_Whitespace_Login(string login)
    {
        var command = new RegisterUserCommand(login, "Qwerty123!");
        var validationResult = await _validator.ValidateAsync(command, CancellationToken.None);
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Login");
    }

    [Fact]
    public async Task Handle_Should_Return_Success()
    {
        var result = await _handler.Handle(Command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
    }
}
