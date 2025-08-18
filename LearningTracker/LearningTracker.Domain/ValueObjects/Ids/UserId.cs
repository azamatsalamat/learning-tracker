using CSharpFunctionalExtensions;

namespace LearningTracker.Domain.ValueObjects.Ids;

public class UserId : SimpleValueObject<Guid> {
    public UserId(Guid value) : base(value) {
    }
}