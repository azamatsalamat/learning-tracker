using CSharpFunctionalExtensions;
using MediatR;

namespace LearningTracker.Application.UseCases.Profile.ParseResume;

public class ParseResumeToProfileCommandHandler : IRequestHandler<ParseResumeToProfileCommand, Result<Domain.Entities.Profile>>
{
    public Task<Result<Domain.Entities.Profile>> Handle(ParseResumeToProfileCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
