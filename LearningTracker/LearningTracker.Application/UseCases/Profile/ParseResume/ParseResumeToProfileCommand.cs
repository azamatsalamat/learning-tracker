using CSharpFunctionalExtensions;
using MediatR;

namespace LearningTracker.Application.UseCases.Profile.ParseResume;

public record ParseResumeToProfileCommand(string Content) : IRequest<Result<Domain.Entities.Profile>>;
