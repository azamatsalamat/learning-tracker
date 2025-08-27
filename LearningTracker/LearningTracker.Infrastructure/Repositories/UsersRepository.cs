using LearningTracker.Domain.Entities;
using LearningTracker.Domain.Repositories;
using LearningTracker.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace LearningTracker.Infrastructure.Repositories;

internal class UsersRepository : IUsersRepository {
    private readonly LearningTrackerDbContext _context;

    public UsersRepository(LearningTrackerDbContext context) {
        _context = context;
    }

    public Task<User?> GetByLogin(string login, CancellationToken ct) {
        return _context.Set<User>().Where(x => x.Login == login).FirstOrDefaultAsync(ct);
    }
}