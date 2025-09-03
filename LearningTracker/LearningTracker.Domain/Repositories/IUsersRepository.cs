using LearningTracker.Domain.Entities;

namespace LearningTracker.Domain.Repositories;

public interface IUsersRepository {
    Task<User?> GetByLogin(string login, CancellationToken ct);
    Task Add(User user, CancellationToken ct);
}