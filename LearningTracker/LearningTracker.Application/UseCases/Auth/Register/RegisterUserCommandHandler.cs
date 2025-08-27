using CSharpFunctionalExtensions;
using MediatR;

namespace LearningTracker.Application.UseCases.Auth.Register;

internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result> {
    public Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}