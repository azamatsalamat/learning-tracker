using LearningTracker.Domain.Entities;

namespace LearningTracker.Api.Services.Base;

public interface ITokenProvider
{
    Task<string> GenerateAccessToken(User user, CancellationToken ct);
}
