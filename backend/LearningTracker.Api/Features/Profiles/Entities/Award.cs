namespace LearningTracker.Features.Profiles.Entities;

public class Award {
    public string Name { get; protected set; }
    public string Issuer { get; protected set; }
    public DateTime Date { get; protected set; }
    public string? Description { get; protected set; }

    public Award(string name, string issuer, DateTime date, string? description) {
        Name = name;
        Issuer = issuer;
        Date = date;
        Description = description;
    }
}
