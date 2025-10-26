using LearningTracker.Features.Profiles.Entities;

namespace LearningTracker.Services;

public interface IResumeParser
{
    Profile Parse(string resumeText);
}
