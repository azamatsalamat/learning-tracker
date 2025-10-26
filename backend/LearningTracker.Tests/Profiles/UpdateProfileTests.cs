using FluentAssertions;
using LearningTracker.Database;
using LearningTracker.Domain.Entities;
using LearningTracker.Domain.ValueObjects;
using LearningTracker.Features.Profiles;
using LearningTracker.Features.Profiles.Entities;
using LearningTracker.Features.Profiles.Enums;
using LearningTracker.Features.Profiles.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace LearningTracker.Tests.Profiles;

public class UpdateProfileTests
{
    #region Validator Tests

    [Fact]
    public void Validator_Should_Fail_When_Id_Is_Empty()
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand() with { Id = Guid.Empty };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProfile.Command.Id));
    }

    [Fact]
    public void Validator_Should_Fail_When_Email_Has_Invalid_Format()
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand() with { Email = "invalid-email" };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProfile.Command.Email));
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name+tag@example.co.uk")]
    [InlineData("user_name@example-domain.com")]
    public void Validator_Should_Pass_When_Email_Has_Valid_Format(string email)
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand() with { Email = email };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateProfile.Command.Email));
    }

    [Fact]
    public void Validator_Should_Pass_When_Email_Is_Null()
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand() with { Email = null };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateProfile.Command.Email));
    }

    [Fact]
    public void Validator_Should_Pass_When_Email_Is_Empty_String()
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand() with { Email = "" };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateProfile.Command.Email));
    }

    [Fact]
    public void Validator_Should_Fail_When_Skills_Is_Null()
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand() with { Skills = null! };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProfile.Command.Skills));
    }

    [Fact]
    public void Validator_Should_Fail_When_Languages_Is_Null()
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand() with { Languages = null! };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProfile.Command.Languages));
    }

    [Fact]
    public void Validator_Should_Fail_When_Experiences_Is_Null()
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand() with { Experiences = null! };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProfile.Command.Experiences));
    }

    [Fact]
    public void Validator_Should_Fail_When_Educations_Is_Null()
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand() with { Educations = null! };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProfile.Command.Educations));
    }

    [Fact]
    public void Validator_Should_Fail_When_PersonalProjects_Is_Null()
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand() with { PersonalProjects = null! };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProfile.Command.PersonalProjects));
    }

    [Fact]
    public void Validator_Should_Fail_When_Certifications_Is_Null()
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand() with { Certifications = null! };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProfile.Command.Certifications));
    }

    [Fact]
    public void Validator_Should_Fail_When_Publications_Is_Null()
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand() with { Publications = null! };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProfile.Command.Publications));
    }

    [Fact]
    public void Validator_Should_Fail_When_Awards_Is_Null()
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand() with { Awards = null! };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProfile.Command.Awards));
    }

    [Fact]
    public void Validator_Should_Pass_When_All_Fields_Are_Valid()
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validator_Should_Pass_When_Arrays_Are_Empty()
    {
        // Arrange
        var validator = new UpdateProfile.Validator();
        var command = CreateValidCommand() with
        {
            Skills = Array.Empty<string>(),
            Languages = Array.Empty<string>(),
            Experiences = Array.Empty<Experience>(),
            Educations = Array.Empty<Education>(),
            PersonalProjects = Array.Empty<PersonalProject>(),
            Certifications = Array.Empty<Certification>(),
            Publications = Array.Empty<Publication>(),
            Awards = Array.Empty<Award>()
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Handler Tests

    [Fact]
    public async Task Handler_Should_Return_Failure_When_Profile_Not_Found()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var handler = new UpdateProfile.Handler(context);
        var command = CreateValidCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Profile not found.");
    }

    [Fact]
    public async Task Handler_Should_Update_Profile_Successfully_When_Profile_Exists()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var existingProfile = CreateProfile();
        context.Set<Profile>().Add(existingProfile);
        await context.SaveChangesAsync();

        var updatedName = new Name("UpdatedFirst", "UpdatedLast");
        var updatedEmail = "updated@example.com";
        var updatedPhone = "+1-555-9999";
        var updatedAddress = new Address("UpdatedCity", "UpdatedCountry");
        var updatedSummary = "Updated professional summary";
        var updatedSkills = new[] { "Python", "Django", "FastAPI" };
        var updatedLanguages = new[] { "English", "Spanish" };

        var command = new UpdateProfile.Command(
            existingProfile.Id,
            updatedName,
            updatedEmail,
            updatedPhone,
            updatedAddress,
            updatedSummary,
            updatedSkills,
            updatedLanguages,
            Array.Empty<Experience>(),
            Array.Empty<Education>(),
            Array.Empty<PersonalProject>(),
            Array.Empty<Certification>(),
            Array.Empty<Publication>(),
            Array.Empty<Award>()
        );

        var handler = new UpdateProfile.Handler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedProfile = await context.Set<Profile>().FirstAsync(p => p.Id == existingProfile.Id);
        updatedProfile.Name.Should().Be(updatedName);
        updatedProfile.Email.Should().Be(updatedEmail);
        updatedProfile.Phone.Should().Be(updatedPhone);
        updatedProfile.Address.Should().Be(updatedAddress);
        updatedProfile.Summary.Should().Be(updatedSummary);
        updatedProfile.Skills.Should().BeEquivalentTo(updatedSkills);
        updatedProfile.Languages.Should().BeEquivalentTo(updatedLanguages);
    }

    [Fact]
    public async Task Handler_Should_Update_Experiences_Successfully()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var existingProfile = CreateProfile();
        context.Set<Profile>().Add(existingProfile);
        await context.SaveChangesAsync();

        var updatedExperiences = new[]
        {
            new Experience(
                "New Company",
                "Senior Developer",
                "Leading development team",
                new DateTime(2022, 1, 1),
                null,
                new[] { "C#", ".NET", "Azure" },
                new[] { "Lead team", "Code reviews" },
                new[] { "Improved performance by 50%" }
            ),
            new Experience(
                "Another Company",
                "Tech Lead",
                "Technical leadership",
                new DateTime(2020, 6, 1),
                new DateTime(2021, 12, 31),
                new[] { "TypeScript", "React" },
                new[] { "Architecture decisions" },
                new[] { "Launched 3 products" }
            )
        };

        var command = CreateValidCommand() with
        {
            Id = existingProfile.Id,
            Experiences = updatedExperiences
        };

        var handler = new UpdateProfile.Handler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedProfile = await context.Set<Profile>().FirstAsync(p => p.Id == existingProfile.Id);
        updatedProfile.Experiences.Should().HaveCount(2);
        updatedProfile.Experiences[0].Company.Should().Be("New Company");
        updatedProfile.Experiences[0].Position.Should().Be("Senior Developer");
        updatedProfile.Experiences[0].EndDate.Should().BeNull();
        updatedProfile.Experiences[1].Company.Should().Be("Another Company");
        updatedProfile.Experiences[1].EndDate.Should().Be(new DateTime(2021, 12, 31));
    }

    [Fact]
    public async Task Handler_Should_Update_Educations_Successfully()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var existingProfile = CreateProfile();
        context.Set<Profile>().Add(existingProfile);
        await context.SaveChangesAsync();

        var updatedEducations = new[]
        {
            new Education(
                "MIT",
                Degree.Master,
                "Computer Science",
                new DateTime(2018, 9, 1),
                new DateTime(2020, 5, 30),
                new[] { "Algorithms", "Machine Learning", "Distributed Systems" },
                new[] { "Dean's List", "Thesis Award" }
            )
        };

        var command = CreateValidCommand() with
        {
            Id = existingProfile.Id,
            Educations = updatedEducations
        };

        var handler = new UpdateProfile.Handler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedProfile = await context.Set<Profile>().FirstAsync(p => p.Id == existingProfile.Id);
        updatedProfile.Educations.Should().HaveCount(1);
        updatedProfile.Educations[0].School.Should().Be("MIT");
        updatedProfile.Educations[0].Degree.Should().Be(Degree.Master);
        updatedProfile.Educations[0].Major.Should().Be("Computer Science");
        updatedProfile.Educations[0].Courses.Should().Contain("Machine Learning");
    }

    [Fact]
    public async Task Handler_Should_Update_PersonalProjects_Successfully()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var existingProfile = CreateProfile();
        context.Set<Profile>().Add(existingProfile);
        await context.SaveChangesAsync();

        var updatedProjects = new[]
        {
            new PersonalProject(
                "AI Chatbot",
                "Built an AI-powered chatbot using GPT",
                new[] { "Python", "OpenAI API", "Flask" }
            ),
            new PersonalProject(
                "E-commerce Platform",
                "Full-stack e-commerce solution",
                new[] { "React", "Node.js", "MongoDB" }
            )
        };

        var command = CreateValidCommand() with
        {
            Id = existingProfile.Id,
            PersonalProjects = updatedProjects
        };

        var handler = new UpdateProfile.Handler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedProfile = await context.Set<Profile>().FirstAsync(p => p.Id == existingProfile.Id);
        updatedProfile.PersonalProjects.Should().HaveCount(2);
        updatedProfile.PersonalProjects[0].Name.Should().Be("AI Chatbot");
        updatedProfile.PersonalProjects[1].Technologies.Should().Contain("MongoDB");
    }

    [Fact]
    public async Task Handler_Should_Update_Certifications_Successfully()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var existingProfile = CreateProfile();
        context.Set<Profile>().Add(existingProfile);
        await context.SaveChangesAsync();

        var updatedCertifications = new[]
        {
            new Certification(
                "AWS Solutions Architect",
                "Amazon Web Services",
                new DateTime(2023, 5, 15),
                new DateTime(2026, 5, 15),
                "AWS-12345",
                "https://aws.amazon.com/verify/12345"
            ),
            new Certification(
                "Azure Developer",
                "Microsoft",
                new DateTime(2023, 8, 1),
                null,
                null,
                null
            )
        };

        var command = CreateValidCommand() with
        {
            Id = existingProfile.Id,
            Certifications = updatedCertifications
        };

        var handler = new UpdateProfile.Handler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedProfile = await context.Set<Profile>().FirstAsync(p => p.Id == existingProfile.Id);
        updatedProfile.Certifications.Should().HaveCount(2);
        updatedProfile.Certifications[0].Name.Should().Be("AWS Solutions Architect");
        updatedProfile.Certifications[0].CredentialId.Should().Be("AWS-12345");
        updatedProfile.Certifications[1].ExpirationDate.Should().BeNull();
    }

    [Fact]
    public async Task Handler_Should_Update_Publications_Successfully()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var existingProfile = CreateProfile();
        context.Set<Profile>().Add(existingProfile);
        await context.SaveChangesAsync();

        var updatedPublications = new[]
        {
            new Publication(
                "Machine Learning in Production",
                "Best practices for deploying ML models",
                new[] { "John Doe", "Jane Smith" },
                "https://journal.example.com/ml-production"
            )
        };

        var command = CreateValidCommand() with
        {
            Id = existingProfile.Id,
            Publications = updatedPublications
        };

        var handler = new UpdateProfile.Handler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedProfile = await context.Set<Profile>().FirstAsync(p => p.Id == existingProfile.Id);
        updatedProfile.Publications.Should().HaveCount(1);
        updatedProfile.Publications[0].Title.Should().Be("Machine Learning in Production");
        updatedProfile.Publications[0].Authors.Should().Contain("Jane Smith");
    }

    [Fact]
    public async Task Handler_Should_Update_Awards_Successfully()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var existingProfile = CreateProfile();
        context.Set<Profile>().Add(existingProfile);
        await context.SaveChangesAsync();

        var updatedAwards = new[]
        {
            new Award(
                "Employee of the Year",
                "Tech Corp",
                new DateTime(2023, 12, 15),
                "Recognized for outstanding contributions"
            ),
            new Award(
                "Hackathon Winner",
                "DevFest 2023",
                new DateTime(2023, 10, 20),
                null
            )
        };

        var command = CreateValidCommand() with
        {
            Id = existingProfile.Id,
            Awards = updatedAwards
        };

        var handler = new UpdateProfile.Handler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedProfile = await context.Set<Profile>().FirstAsync(p => p.Id == existingProfile.Id);
        updatedProfile.Awards.Should().HaveCount(2);
        updatedProfile.Awards[0].Name.Should().Be("Employee of the Year");
        updatedProfile.Awards[1].Description.Should().BeNull();
    }

    [Fact]
    public async Task Handler_Should_Update_All_Collections_To_Empty_Arrays()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var existingProfile = CreateProfile();
        context.Set<Profile>().Add(existingProfile);
        await context.SaveChangesAsync();

        var command = CreateValidCommand() with
        {
            Id = existingProfile.Id,
            Skills = Array.Empty<string>(),
            Languages = Array.Empty<string>(),
            Experiences = Array.Empty<Experience>(),
            Educations = Array.Empty<Education>(),
            PersonalProjects = Array.Empty<PersonalProject>(),
            Certifications = Array.Empty<Certification>(),
            Publications = Array.Empty<Publication>(),
            Awards = Array.Empty<Award>()
        };

        var handler = new UpdateProfile.Handler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedProfile = await context.Set<Profile>().FirstAsync(p => p.Id == existingProfile.Id);
        updatedProfile.Skills.Should().BeEmpty();
        updatedProfile.Languages.Should().BeEmpty();
        updatedProfile.Experiences.Should().BeEmpty();
        updatedProfile.Educations.Should().BeEmpty();
        updatedProfile.PersonalProjects.Should().BeEmpty();
        updatedProfile.Certifications.Should().BeEmpty();
        updatedProfile.Publications.Should().BeEmpty();
        updatedProfile.Awards.Should().BeEmpty();
    }

    [Fact]
    public async Task Handler_Should_Update_Nullable_Fields_To_Null()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var existingProfile = CreateProfile();
        context.Set<Profile>().Add(existingProfile);
        await context.SaveChangesAsync();

        var command = CreateValidCommand() with
        {
            Id = existingProfile.Id,
            Name = null,
            Email = null,
            Phone = null,
            Address = null,
            Summary = null
        };

        var handler = new UpdateProfile.Handler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedProfile = await context.Set<Profile>().FirstAsync(p => p.Id == existingProfile.Id);
        updatedProfile.Name.Should().BeNull();
        updatedProfile.Email.Should().BeNull();
        updatedProfile.Phone.Should().BeNull();
        updatedProfile.Address.Should().BeNull();
        updatedProfile.Summary.Should().BeNull();
    }

    [Fact]
    public async Task Handler_Should_Persist_Changes_To_Database()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var existingProfile = CreateProfile();
        context.Set<Profile>().Add(existingProfile);
        await context.SaveChangesAsync();

        var command = CreateValidCommand() with
        {
            Id = existingProfile.Id,
            Email = "persisted@example.com"
        };

        var handler = new UpdateProfile.Handler(context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        context.Entry(existingProfile).State = EntityState.Detached;
        var persistedProfile = await context.Set<Profile>().FirstOrDefaultAsync(p => p.Id == existingProfile.Id);
        persistedProfile.Should().NotBeNull();
        persistedProfile!.Email.Should().Be("persisted@example.com");
    }

    [Fact]
    public async Task Handler_Should_Not_Modify_CreationDate_Or_Id()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var existingProfile = CreateProfile();
        var originalId = existingProfile.Id;
        var originalCreationDate = existingProfile.CreationDate;

        context.Set<Profile>().Add(existingProfile);
        await context.SaveChangesAsync();

        var command = CreateValidCommand() with { Id = existingProfile.Id };
        var handler = new UpdateProfile.Handler(context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedProfile = await context.Set<Profile>().FirstAsync(p => p.Id == existingProfile.Id);
        updatedProfile.Id.Should().Be(originalId);
        updatedProfile.CreationDate.Should().Be(originalCreationDate);
    }

    [Fact]
    public async Task Handler_Should_Handle_Complex_Profile_Update()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var existingProfile = CreateProfile();
        context.Set<Profile>().Add(existingProfile);
        await context.SaveChangesAsync();

        var command = new UpdateProfile.Command(
            existingProfile.Id,
            new Name("Complex", "Update"),
            "complex@example.com",
            "+1-555-1234",
            new Address("San Francisco", "USA"),
            "Comprehensive professional with diverse experience",
            new[] { "C#", "Python", "JavaScript", "Go", "Rust" },
            new[] { "English", "Spanish", "French" },
            new[]
            {
                new Experience("Company A", "Position A", "Description A", DateTime.Now.AddYears(-5), DateTime.Now.AddYears(-3), new[] { "Tech A" }, new[] { "Resp A" }, new[] { "Ach A" }),
                new Experience("Company B", "Position B", "Description B", DateTime.Now.AddYears(-3), null, new[] { "Tech B" }, new[] { "Resp B" }, new[] { "Ach B" })
            },
            new[]
            {
                new Education("University A", Degree.Bachelor, "CS", DateTime.Now.AddYears(-10), DateTime.Now.AddYears(-6), new[] { "Course A" }, new[] { "Achievement A" }),
                new Education("University B", Degree.Master, "SE", DateTime.Now.AddYears(-6), DateTime.Now.AddYears(-4), new[] { "Course B" }, new[] { "Achievement B" })
            },
            new[]
            {
                new PersonalProject("Project A", "Desc A", new[] { "Tech A" }),
                new PersonalProject("Project B", "Desc B", new[] { "Tech B" })
            },
            new[]
            {
                new Certification("Cert A", "Issuer A", DateTime.Now.AddYears(-2), DateTime.Now.AddYears(1), "ID-A", "https://cert-a.com")
            },
            new[]
            {
                new Publication("Publication A", "Desc A", new[] { "Author A", "Author B" }, "https://pub-a.com")
            },
            new[]
            {
                new Award("Award A", "Issuer A", DateTime.Now.AddYears(-1), "Award Description")
            }
        );

        var handler = new UpdateProfile.Handler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedProfile = await context.Set<Profile>().FirstAsync(p => p.Id == existingProfile.Id);
        updatedProfile.Name.Should().Be(new Name("Complex", "Update"));
        updatedProfile.Skills.Should().HaveCount(5);
        updatedProfile.Languages.Should().HaveCount(3);
        updatedProfile.Experiences.Should().HaveCount(2);
        updatedProfile.Educations.Should().HaveCount(2);
        updatedProfile.PersonalProjects.Should().HaveCount(2);
        updatedProfile.Certifications.Should().HaveCount(1);
        updatedProfile.Publications.Should().HaveCount(1);
        updatedProfile.Awards.Should().HaveCount(1);
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

    private static UpdateProfile.Command CreateValidCommand()
    {
        return new UpdateProfile.Command(
            Guid.NewGuid(),
            new Name("John", "Doe"),
            "john.doe@example.com",
            "+1-555-1234",
            new Address("New York", "USA"),
            "Experienced software developer",
            new[] { "C#", ".NET", "ASP.NET Core" },
            new[] { "English", "French" },
            Array.Empty<Experience>(),
            Array.Empty<Education>(),
            Array.Empty<PersonalProject>(),
            Array.Empty<Certification>(),
            Array.Empty<Publication>(),
            Array.Empty<Award>()
        );
    }

    private static Profile CreateProfile()
    {
        return new Profile(
            new Name("Original", "Name"),
            "original@example.com",
            "+1-555-0000",
            new Address("OldCity", "OldCountry"),
            "Original summary",
            new[] { "Java", "Spring" },
            new[] { "English" },
            Array.Empty<Experience>(),
            Array.Empty<Education>(),
            Array.Empty<PersonalProject>(),
            Array.Empty<Certification>(),
            Array.Empty<Publication>(),
            Array.Empty<Award>()
        );
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
