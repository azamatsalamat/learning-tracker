using CSharpFunctionalExtensions;
using MediatR;

namespace LearningTracker.Application.UseCases.Auth.Register;

public record RegisterUserCommand(string Login, string Password) : IRequest<Result>;