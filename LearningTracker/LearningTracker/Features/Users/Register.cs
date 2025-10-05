using CSharpFunctionalExtensions;
using FluentValidation;
using LearningTracker.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningTracker.Features.Users;

public record RegisterRequest(string Login, string Password);

public static class Register
{
    public record Command(string Login, string Password) : IRequest<Result>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Login).NotEmpty().MinimumLength(1).MaximumLength(100);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(255)
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*.()])[a-zA-Z0-9!@#$%^&*.()]+$")
                .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character (!@#$%^&*.())");
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly LearningTrackerDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public Handler(LearningTrackerDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result> Handle(Command request, CancellationToken ct)
        {
            var userWithSameLogin = await _context.Set<User>()
                .Where(x => x.Login == request.Login).FirstOrDefaultAsync(ct);
            if (userWithSameLogin != null)
            {
                return Result.Failure("User with this login already exists.");
            }

            var encryptedPassword = _passwordHasher.HashPassword(null!, request.Password);
            var user = new User(request.Login, encryptedPassword);

            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
