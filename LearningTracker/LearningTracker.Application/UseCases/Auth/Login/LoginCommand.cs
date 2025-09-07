using CSharpFunctionalExtensions;
using LearningTracker.Domain.Entities;
using MediatR;

namespace LearningTracker.Application.UseCases.Auth.Login;

public record LoginCommand(string Login, string Password) : IRequest<Result<User>>;
