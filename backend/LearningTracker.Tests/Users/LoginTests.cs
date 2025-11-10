using FluentAssertions;
using LearningTracker.Database;
using LearningTracker.Entities;
using LearningTracker.Features.Profiles.ValueObjects;
using LearningTracker.Features.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningTracker.Tests.Users;

public class LoginTests
{
    #region Validator Tests

    [Fact]
    public void Validator_Should_Fail_When_Login_Is_Empty()
    {
        // Arrange
        var validator = new Login.Validator();
        var command = new Login.Command("", "password");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Login.Command.Login));
    }

    [Fact]
    public void Validator_Should_Fail_When_Password_Is_Empty()
    {
        // Arrange
        var validator = new Login.Validator();
        var command = new Login.Command("user", "");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Login.Command.Password));
    }

    [Fact]
    public void Validator_Should_Fail_When_Both_Login_And_Password_Are_Empty()
    {
        // Arrange
        var validator = new Login.Validator();
        var command = new Login.Command("", "");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Login.Command.Login));
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Login.Command.Password));
    }

    [Fact]
    public void Validator_Should_Pass_When_Both_Login_And_Password_Are_Provided()
    {
        // Arrange
        var validator = new Login.Validator();
        var command = new Login.Command("user", "password");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Handler Tests

    [Fact]
    public async Task Handler_Should_Return_Failure_When_User_Does_Not_Exist()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var passwordHasher = new PasswordHasher<User>();
        var handler = new Login.Handler(passwordHasher, context);
        var command = new Login.Command("nonexistentuser", "Password123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Invalid login or password");
    }

    [Fact]
    public async Task Handler_Should_Return_Failure_When_Password_Is_Incorrect()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var passwordHasher = new PasswordHasher<User>();

        var correctPassword = "Password123!";
        var hashedPassword = passwordHasher.HashPassword(null!, correctPassword);
        var user = new User("testuser", hashedPassword);
        context.Set<User>().Add(user);
        await context.SaveChangesAsync();

        var handler = new Login.Handler(passwordHasher, context);
        var command = new Login.Command("testuser", "WrongPassword123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Invalid login or password");
    }

    [Fact]
    public async Task Handler_Should_Return_Success_With_User_When_Credentials_Are_Valid()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var passwordHasher = new PasswordHasher<User>();

        var password = "Password123!";
        var hashedPassword = passwordHasher.HashPassword(null!, password);
        var user = new User("validuser", hashedPassword);
        context.Set<User>().Add(user);
        await context.SaveChangesAsync();

        var handler = new Login.Handler(passwordHasher, context);
        var command = new Login.Command("validuser", password);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Login.Should().Be("validuser");
        result.Value.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task Handler_Should_Return_Correct_User_Object()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var passwordHasher = new PasswordHasher<User>();

        var password = "Password123!";
        var hashedPassword = passwordHasher.HashPassword(null!, password);
        var user = new User("myuser", hashedPassword);
        context.Set<User>().Add(user);
        await context.SaveChangesAsync();

        var handler = new Login.Handler(passwordHasher, context);
        var command = new Login.Command("myuser", password);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Login.Should().Be("myuser");
        result.Value.Password.Should().Be(hashedPassword);
        result.Value.Id.Should().NotBe(Guid.Empty);
        result.Value.CreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handler_Should_Handle_Case_Sensitive_Login()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var passwordHasher = new PasswordHasher<User>();

        var password = "Password123!";
        var hashedPassword = passwordHasher.HashPassword(null!, password);
        var user = new User("CaseSensitiveUser", hashedPassword);
        context.Set<User>().Add(user);
        await context.SaveChangesAsync();

        var handler = new Login.Handler(passwordHasher, context);
        var command = new Login.Command("casesensitiveuser", password);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        // This test documents current behavior - logins are case-sensitive
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Invalid login or password");
    }

    [Fact]
    public async Task Handler_Should_Verify_Password_Using_PasswordHasher()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var passwordHasher = new PasswordHasher<User>();

        var password = "MySecurePass1!";
        var hashedPassword = passwordHasher.HashPassword(null!, password);
        var user = new User("secureuser", hashedPassword);
        context.Set<User>().Add(user);
        await context.SaveChangesAsync();

        var handler = new Login.Handler(passwordHasher, context);

        // Act
        var successResult = await handler.Handle(new Login.Command("secureuser", password), CancellationToken.None);
        var failureResult = await handler.Handle(new Login.Command("secureuser", "WrongPassword1!"), CancellationToken.None);

        // Assert
        successResult.IsSuccess.Should().BeTrue();
        failureResult.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handler_Should_Return_Same_Error_Message_For_Invalid_Login_And_Password()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var passwordHasher = new PasswordHasher<User>();

        var password = "Password123!";
        var hashedPassword = passwordHasher.HashPassword(null!, password);
        var user = new User("existinguser", hashedPassword);
        context.Set<User>().Add(user);
        await context.SaveChangesAsync();

        var handler = new Login.Handler(passwordHasher, context);

        // Act
        var invalidUserResult = await handler.Handle(new Login.Command("nonexistent", "Password123!"), CancellationToken.None);
        var invalidPasswordResult = await handler.Handle(new Login.Command("existinguser", "WrongPassword123!"), CancellationToken.None);

        // Assert
        // Both should return the same error message for security reasons
        invalidUserResult.IsFailure.Should().BeTrue();
        invalidPasswordResult.IsFailure.Should().BeTrue();
        invalidUserResult.Error.Should().Be(invalidPasswordResult.Error);
    }

    [Fact]
    public async Task Handler_Should_Authenticate_Multiple_Different_Users()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var passwordHasher = new PasswordHasher<User>();

        var user1 = new User("user1", passwordHasher.HashPassword(null!, "Password1!"));
        var user2 = new User("user2", passwordHasher.HashPassword(null!, "Password2!"));
        var user3 = new User("user3", passwordHasher.HashPassword(null!, "Password3!"));

        context.Set<User>().AddRange(user1, user2, user3);
        await context.SaveChangesAsync();

        var handler = new Login.Handler(passwordHasher, context);

        // Act
        var result1 = await handler.Handle(new Login.Command("user1", "Password1!"), CancellationToken.None);
        var result2 = await handler.Handle(new Login.Command("user2", "Password2!"), CancellationToken.None);
        var result3 = await handler.Handle(new Login.Command("user3", "Password3!"), CancellationToken.None);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result1.Value.Login.Should().Be("user1");

        result2.IsSuccess.Should().BeTrue();
        result2.Value.Login.Should().Be("user2");

        result3.IsSuccess.Should().BeTrue();
        result3.Value.Login.Should().Be("user3");
    }

    #endregion

    #region Helper Methods

    private static TestLearningTrackerDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<LearningTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new TestLearningTrackerDbContext(options);
    }

    #endregion

    #region Test DbContext

    private class TestLearningTrackerDbContext : LearningTrackerDbContext
    {
        public TestLearningTrackerDbContext(DbContextOptions<LearningTrackerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Override jsonb configurations for InMemory database
            var profileEntity = modelBuilder.Entity<Profile>();

            profileEntity.Property(p => p.Skills)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<string[]>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? Array.Empty<string>());

            profileEntity.Property(p => p.Languages)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<string[]>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? Array.Empty<string>());

            profileEntity.Property(p => p.Experiences)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Experience[]>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? Array.Empty<Experience>());

            profileEntity.Property(p => p.Educations)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Education[]>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? Array.Empty<Education>());

            profileEntity.Property(p => p.PersonalProjects)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<PersonalProject[]>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? Array.Empty<PersonalProject>());

            profileEntity.Property(p => p.Certifications)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Certification[]>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? Array.Empty<Certification>());

            profileEntity.Property(p => p.Publications)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Publication[]>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? Array.Empty<Publication>());

            profileEntity.Property(p => p.Awards)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Award[]>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? Array.Empty<Award>());
        }
    }

    #endregion
}
