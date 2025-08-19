using LearningTracker.Domain.Enums;

namespace LearningTracker.Domain.Entities;

public class Education {
    public string School { get; protected set; }
    public Degree Degree { get; protected set; }
    public string Major { get; protected set; }
    public DateTime StartDate { get; protected set; }
    public DateTime? EndDate { get; protected set; }
    public string[] Courses { get; protected set; }
    public string[] Achievements { get; protected set; }

    public Education(string school, Degree degree, string major, DateTime startDate, DateTime? endDate, 
        string[] courses, string[] achievements) {
        School = school;
        Degree = degree;
        Major = major;
        StartDate = startDate;
        EndDate = endDate;
        Courses = courses;
        Achievements = achievements;
    }
}