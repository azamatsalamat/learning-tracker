using FluentValidation;
using LearningTracker.Application.Configuration.Pipelines;

namespace LearningTracker.Application.UseCases.Profile.ParseCv;

internal class ParseCvToProfileCommandValidator : ValidationBehavior<ParseCvToProfileCommand, Domain.Entities.Profile>
{
    public ParseCvToProfileCommandValidator()
    {
        RuleFor(x => x.File).NotNull();
    }
}
