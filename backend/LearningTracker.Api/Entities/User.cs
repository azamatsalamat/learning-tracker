using SequentialGuid;

namespace LearningTracker.Entities;

public class User {
    public Guid Id { get; protected set; }
    public DateTime CreationDate { get; protected set; }
    public string Login { get; protected set; }
    public string Password { get; protected set; }

    protected User() { }

    public User(string login, string password) {
        Id = SequentialGuidGenerator.Instance.NewGuid();
        CreationDate = DateTime.UtcNow;
        Login = login;
        Password = password;
    }
}
