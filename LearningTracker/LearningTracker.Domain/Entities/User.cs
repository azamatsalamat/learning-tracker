using LearningTracker.Domain.ValueObjects.Ids;

namespace LearningTracker.Domain.Entities;

public class User {
    public UserId Id { get; protected set; }
    public DateTime CreationDate { get; protected set; }
    public string Login { get; protected set; }
    public string Password { get; protected set; }
}