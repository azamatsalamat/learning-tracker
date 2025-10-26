using CSharpFunctionalExtensions;
using FluentValidation;
using LearningTracker.Database;
using LearningTracker.Features.Profiles.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningTracker.Features.Profiles;

public static class GetProfile
{
    public record Query(Guid Id) : IRequest<Result<Profile>>;

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Query, Result<Profile>>
    {
        private readonly LearningTrackerDbContext _context;

        public Handler(LearningTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Profile>> Handle(Query request, CancellationToken ct)
        {
            var profile = await _context.Set<Profile>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (profile == null)
            {
                return Result.Failure<Profile>("Profile not found");
            }

            return Result.Success(profile);
        }
    }
}
