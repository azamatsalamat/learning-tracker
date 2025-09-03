using LearningTracker.Domain.ValueObjects.Ids;
using SequentialGuid;

namespace LearningTracker.Domain.Entities;

public class User {
    public UserId Id { get; protected set; }
    public DateTime CreationDate { get; protected set; }
    public string Login { get; protected set; }
    public string Password { get; protected set; }

    protected User() { }

    public User(string login, string password) {
        Id = new UserId(SequentialGuidGenerator.Instance.NewGuid());
        CreationDate = DateTime.UtcNow;
        Login = login;
        Password = password;
    }
}