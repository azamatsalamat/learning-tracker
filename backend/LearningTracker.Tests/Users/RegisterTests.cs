using FluentAssertions;
using LearningTracker.Database;
using LearningTracker.Entities;
using LearningTracker.Features.Profiles.ValueObjects;
using LearningTracker.Features.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningTracker.Tests.Users;

public class RegisterTests
{
    #region Validator Tests

    [Fact]
    public void Validator_Should_Fail_When_Login_Is_Empty()
    {
        // Arrange
        var validator = new Register.Validator();
        var command = new Register.Command("", "Password123!");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Register.Command.Login));
    }

    [Fact]
    public void Validator_Should_Fail_When_Login_Exceeds_Maximum_Length()
    {
        // Arrange
        var validator = new Register.Validator();
        var command = new Register.Command(new string('a', 101), "Password123!");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Register.Command.Login));
    }

    [Theory]
    [InlineData("user")]
    [InlineData("admin")]
    [InlineData("testuser123")]
    public void Validator_Should_Pass_When_Login_Has_Valid_Length(string login)
    {
        // Arrange
        var validator = new Register.Validator();
        var command = new Register.Command(login, "Password123!");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == nameof(Register.Command.Login));
    }

    [Fact]
    public void Validator_Should_Fail_When_Password_Is_Empty()
    {
        // Arrange
        var validator = new Register.Validator();
        var command = new Register.Command("user", "");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Register.Command.Password));
    }

    [Fact]
    public void Validator_Should_Fail_When_Password_Is_Too_Short()
    {
        // Arrange
        var validator = new Register.Validator();
        var command = new Register.Command("user", "Pass1!");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Register.Command.Password));
    }

    [Fact]
    public void Validator_Should_Fail_When_Password_Exceeds_Maximum_Length()
    {
        // Arrange
        var validator = new Register.Validator();
        var command = new Register.Command("user", new string('a', 256));

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Register.Command.Password));
    }

    [Theory]
    [InlineData("password123")] // No uppercase
    [InlineData("PASSWORD123")] // No lowercase
    [InlineData("Password")] // No digit
    [InlineData("Password123")] // No special character
    [InlineData("Pass word123!")] // Contains space
    [InlineData("Password123~")] // Invalid special character
    public void Validator_Should_Fail_When_Password_Does_Not_Match_Pattern(string password)
    {
        // Arrange
        var validator = new Register.Validator();
        var command = new Register.Command("user", password);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Register.Command.Password));
    }

    [Theory]
    [InlineData("Password123!")]
    [InlineData("MySecure1@")]
    [InlineData("Complex9$Password")]
    [InlineData("Test1234%")]
    [InlineData("Abc12345&")]
    public void Validator_Should_Pass_When_Password_Matches_Pattern(string password)
    {
        // Arrange
        var validator = new Register.Validator();
        var command = new Register.Command("user", password);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == nameof(Register.Command.Password));
    }

    [Fact]
    public void Validator_Should_Pass_When_All_Fields_Are_Valid()
    {
        // Arrange
        var validator = new Register.Validator();
        var command = new Register.Command("testuser", "Password123!");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Handler Tests

    [Fact]
    public async Task Handler_Should_Register_User_Successfully()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var passwordHasher = new PasswordHasher<User>();
        var handler = new Register.Handler(context, passwordHasher);
        var command = new Register.Command("newuser", "Password123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var user = await context.Set<User>().FirstOrDefaultAsync(u => u.Login == "newuser");
        user.Should().NotBeNull();
        user!.Login.Should().Be("newuser");
        user.Password.Should().NotBe("Password123!"); // Password should be hashed
    }

    [Fact]
    public async Task Handler_Should_Hash_Password_When_Registering()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var passwordHasher = new PasswordHasher<User>();
        var handler = new Register.Handler(context, passwordHasher);
        var command = new Register.Command("testuser", "Password123!");

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var user = await context.Set<User>().FirstAsync(u => u.Login == "testuser");
        user.Password.Should().NotBe("Password123!");

        // Verify password can be verified with the hasher
        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, "Password123!");
        verificationResult.Should().Be(PasswordVerificationResult.Success);
    }

    [Fact]
    public async Task Handler_Should_Return_Failure_When_User_With_Same_Login_Exists()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var passwordHasher = new PasswordHasher<User>();
        var existingUser = new User("existinguser", "hashedpassword");
        context.Set<User>().Add(existingUser);
        await context.SaveChangesAsync();

        var handler = new Register.Handler(context, passwordHasher);
        var command = new Register.Command("existinguser", "Password123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User with this login already exists.");
    }

    [Fact]
    public async Task Handler_Should_Persist_User_To_Database()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var passwordHasher = new PasswordHasher<User>();
        var handler = new Register.Handler(context, passwordHasher);
        var command = new Register.Command("persisteduser", "Password123!");

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var userCount = await context.Set<User>().CountAsync(u => u.Login == "persisteduser");
        userCount.Should().Be(1);

        var user = await context.Set<User>().FirstAsync(u => u.Login == "persisteduser");
        user.Id.Should().NotBe(Guid.Empty);
        user.CreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handler_Should_Create_User_With_Generated_Id_And_CreationDate()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var passwordHasher = new PasswordHasher<User>();
        var handler = new Register.Handler(context, passwordHasher);
        var command = new Register.Command("newuser", "Password123!");

        var beforeCreation = DateTime.UtcNow;

        // Act
        await handler.Handle(command, CancellationToken.None);

        var afterCreation = DateTime.UtcNow;

        // Assert
        var user = await context.Set<User>().FirstAsync(u => u.Login == "newuser");
        user.Id.Should().NotBe(Guid.Empty);
        user.CreationDate.Should().BeOnOrAfter(beforeCreation);
        user.CreationDate.Should().BeOnOrBefore(afterCreation);
    }

    [Fact]
    public async Task Handler_Should_Allow_Multiple_Users_With_Different_Logins()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var passwordHasher = new PasswordHasher<User>();
        var handler = new Register.Handler(context, passwordHasher);

        // Act
        var result1 = await handler.Handle(new Register.Command("user1", "Password123!"), CancellationToken.None);
        var result2 = await handler.Handle(new Register.Command("user2", "Password456!"), CancellationToken.None);
        var result3 = await handler.Handle(new Register.Command("user3", "Password789!"), CancellationToken.None);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();
        result3.IsSuccess.Should().BeTrue();

        var userCount = await context.Set<User>().CountAsync();
        userCount.Should().Be(3);
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
