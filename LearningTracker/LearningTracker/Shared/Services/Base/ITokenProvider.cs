using LearningTracker.Features.Users;

namespace LearningTracker.Shared.Services.Base;

public interface ITokenProvider
{
    Task<string> GenerateAccessToken(User user, CancellationToken ct);
}
