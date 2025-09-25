using FluentValidation;
using LearningTracker.Application.Configuration.Pipelines;

namespace LearningTracker.Application.UseCases.Profile.ParseResume;

internal class ParseResumeToProfileCommandValidator : ValidationBehavior<ParseResumeToProfileCommand, Domain.Entities.Profile>
{
    public ParseResumeToProfileCommandValidator()
    {
        RuleFor(x => x.Content).NotNull().NotEmpty();
    }
}
