namespace LearningTracker.Domain.Entities;

public class Experience {
    public string Company { get; protected set; }
    public string Position { get; protected set; }
    public string Description { get; protected set; }
    public DateTime StartDate { get; protected set; }
    public DateTime? EndDate { get; protected set; }
    public string[] Technologies { get; protected set; }
    public string[] Responsibilities { get; protected set; }
    public string[] Achievements { get; protected set; }

    public Experience(string company, string position, string description, DateTime startDate, DateTime? endDate, 
        string[] technologies, string[] responsibilities, string[] achievements) {
        Company = company;
        Position = position;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Technologies = technologies;
        Responsibilities = responsibilities;
        Achievements = achievements;
    }
}