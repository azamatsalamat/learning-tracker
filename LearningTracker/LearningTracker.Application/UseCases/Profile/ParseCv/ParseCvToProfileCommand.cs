using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LearningTracker.Application.UseCases.Profile.ParseCv;

public record ParseCvToProfileCommand(string Content) : IRequest<Result<Domain.Entities.Profile>>;
