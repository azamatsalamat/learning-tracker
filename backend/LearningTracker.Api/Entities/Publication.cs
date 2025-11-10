namespace LearningTracker.Entities;

public class Publication {
    public string Title { get; protected set; }
    public string Description { get; protected set; }
    public string[] Authors { get; protected set; }
    public string? Link { get; protected set; }

    public Publication(string title, string description, string[] authors, string? link) {
        Title = title;
        Description = description;
        Authors = authors;
        Link = link;
    }
}
