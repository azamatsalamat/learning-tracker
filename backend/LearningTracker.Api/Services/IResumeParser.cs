using LearningTracker.Entities;

namespace LearningTracker.Services;

public interface IResumeParser
{
    Profile Parse(string resumeText);
}
