using LearningTracker.Entities;
using LearningTracker.Features.Users;

namespace LearningTracker.Services.Base;

public interface ITokenProvider
{
    Task<string> GenerateAccessToken(User user, CancellationToken ct);
}
