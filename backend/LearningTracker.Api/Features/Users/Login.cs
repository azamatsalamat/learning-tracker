using CSharpFunctionalExtensions;
using FluentValidation;
using LearningTracker.Database;
using LearningTracker.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningTracker.Features.Users;

public record LoginRequest(string Login, string Password);

public record LoginResponse(string AccessToken, bool HasProfile);

public static class Login
{
    public record Command(string Login, string Password) : IRequest<Result<User>>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<User>>
    {
        private readonly LearningTrackerDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public Handler(IPasswordHasher<User> passwordHasher, LearningTrackerDbContext context)
        {
            _passwordHasher = passwordHasher;
            _context = context;
        }

        public async Task<Result<User>> Handle(Command request, CancellationToken ct)
        {
            var user = await _context.Set<User>()
                .Include(x => x.Profile)
                .Where(x => x.Login == request.Login)
                .FirstOrDefaultAsync(ct);
            if (user == null) {
                return Result.Failure<User>("Invalid login or password");
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed) {
                return Result.Failure<User>("Invalid login or password");
            }

            return Result.Success(user);
        }
    }
}
