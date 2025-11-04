namespace LearningTracker.Entities;

public class PersonalProject {
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public string[] Technologies { get; protected set; }

    public PersonalProject(string name, string description, string[] technologies) {
        Name = name;
        Description = description;
        Technologies = technologies;
    }
}
