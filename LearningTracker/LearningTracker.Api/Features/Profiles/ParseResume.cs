using CSharpFunctionalExtensions;
using FluentValidation;
using LearningTracker.Database;
using LearningTracker.Features.Profiles.Entities;
using LearningTracker.Services;
using MediatR;

namespace LearningTracker.Features.Profiles;

public static class ParseResume
{
    public record Command(string Content) : IRequest<Result<Profile>>;

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Content).NotNull().NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<Profile>>
    {
        private readonly IResumeParser _resumeParser;
        private readonly LearningTrackerDbContext _dbContext;

        public Handler(IResumeParser resumeParser, LearningTrackerDbContext dbContext)
        {
            _resumeParser = resumeParser;
            _dbContext = dbContext;
        }

        public async Task<Result<Profile>> Handle(Command request, CancellationToken ct)
        {
            var profile = _resumeParser.Parse(request.Content);

            _dbContext.Set<Profile>().Add(profile);
            await _dbContext.SaveChangesAsync(ct);

            return Result.Success(profile);
        }
    }
}
