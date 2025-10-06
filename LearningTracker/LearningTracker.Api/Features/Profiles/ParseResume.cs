using CSharpFunctionalExtensions;
using FluentValidation;
using LearningTracker.Features.Profiles.Entities;
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

    internal class Handler : IRequestHandler<Command, Result<Profile>>
    {
        public Task<Result<Profile>> Handle(Command request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
