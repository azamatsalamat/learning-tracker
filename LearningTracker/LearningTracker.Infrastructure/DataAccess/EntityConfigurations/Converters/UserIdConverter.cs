using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using LearningTracker.Domain.ValueObjects.Ids;

namespace LearningTracker.Infrastructure.DataAccess;

public class UserIdConverter : ValueConverter<UserId, Guid>
{
    public UserIdConverter() : base(
        userId => userId.Value,
        guid => new UserId(guid))
    {
    }
}

