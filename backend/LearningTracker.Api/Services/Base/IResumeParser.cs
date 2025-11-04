using LearningTracker.Entities;

namespace LearningTracker.Services.Base;

public interface IResumeParser
{
    Profile Parse(Guid userId, string resumeText);
}
