using CSharpFunctionalExtensions;
using FluentValidation;
using LearningTracker.Database;
using LearningTracker.Domain.ValueObjects;
using LearningTracker.Entities;
using LearningTracker.Features.Profiles.Enums;
using LearningTracker.Features.Profiles.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningTracker.Features.Profiles;

public record UpdateProfileRequest(
    Guid Id,
    NameDto? Name,
    string? Email,
    string? Phone,
    AddressDto? Address,
    string? Summary,
    string[]? Skills,
    string[]? Languages,
    ExperienceDto[]? Experiences,
    EducationDto[]? Educations,
    PersonalProjectDto[]? PersonalProjects,
    CertificationDto[]? Certifications,
    PublicationDto[]? Publications,
    AwardDto[]? Awards
);

public record NameDto(string FirstName, string LastName);
public record AddressDto(string City, string Country);
public record ExperienceDto(string Company, string Position, string Description, DateTime StartDate, DateTime? EndDate, string[]? Technologies, string[]? Responsibilities, string[]? Achievements);
public record EducationDto(string School, Degree Degree, string Major, DateTime StartDate, DateTime? EndDate, string[]? Courses, string[]? Achievements);
public record PersonalProjectDto(string Name, string Description, string[]? Technologies);
public record CertificationDto(string Name, string Issuer, DateTime IssueDate, DateTime? ExpirationDate, string? CredentialId, string? CredentialUrl);
public record PublicationDto(string Title, string Description, string[]? Authors, string? Link);
public record AwardDto(string Name, string Issuer, DateTime Date, string? Description);

public static class UpdateProfile
{
    public record Command(
        Guid Id,
        Name? Name,
        string? Email,
        string? Phone,
        Address? Address,
        string? Summary,
        string[] Skills,
        string[] Languages,
        Experience[] Experiences,
        Education[] Educations,
        PersonalProject[] PersonalProjects,
        Certification[] Certifications,
        Publication[] Publications,
        Award[] Awards
    ) : IRequest<Result>;

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
            RuleFor(x => x.Skills).NotNull();
            RuleFor(x => x.Languages).NotNull();
            RuleFor(x => x.Experiences).NotNull();
            RuleFor(x => x.Educations).NotNull();
            RuleFor(x => x.PersonalProjects).NotNull();
            RuleFor(x => x.Certifications).NotNull();
            RuleFor(x => x.Publications).NotNull();
            RuleFor(x => x.Awards).NotNull();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly LearningTrackerDbContext _context;

        public Handler(LearningTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(Command request, CancellationToken ct)
        {
            var profile = await _context.Set<Profile>()
                .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

            if (profile == null)
            {
                return Result.Failure("Profile not found.");
            }

            profile.Edit(
                request.Name,
                request.Email,
                request.Phone,
                request.Address,
                request.Summary,
                request.Skills,
                request.Languages,
                request.Experiences,
                request.Educations,
                request.PersonalProjects,
                request.Certifications,
                request.Publications,
                request.Awards
            );

            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
