using LearningTracker.Domain.ValueObjects.Ids;

namespace LearningTracker.Domain.Entities;

public class Profile {
    public UserId Id { get; protected set; }
    public DateTime CreationDate { get; protected set; }
}