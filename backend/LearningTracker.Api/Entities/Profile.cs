using LearningTracker.Domain.ValueObjects;
using LearningTracker.Features.Profiles.ValueObjects;

namespace LearningTracker.Entities;

public class Profile {
    public Guid Id { get; protected set; }
    public virtual User User { get; protected set; }
    public DateTime CreationDate { get; protected set; }
    public Name? Name { get; protected set; }
    public string? Email { get; protected set; }
    public string? Phone { get; protected set; }
    public Address? Address { get; protected set; }
    public string? Summary { get; protected set; }
    public string[] Skills { get; protected set; }
    public string[] Languages { get; protected set; }
    public Experience[] Experiences { get; protected set; }
    public Education[] Educations { get; protected set; }
    public PersonalProject[] PersonalProjects {get; protected set; }
    public Certification[] Certifications { get; protected set; }
    public Publication[] Publications { get; protected set; }
    public Award[] Awards { get; protected set; }

    protected Profile() {}

    public Profile(Guid userId, Name? name, string? email, string? phone, Address? address, string? summary, string[] skills,
        string[] languages, Experience[] experiences, Education[] educations, PersonalProject[] personalProjects,
        Certification[] certifications, Publication[] publications, Award[] awards) {
        Id = userId;
        CreationDate = DateTime.UtcNow;
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        Summary = summary;
        Skills = skills;
        Languages = languages;
        Experiences = experiences;
        Educations = educations;
        PersonalProjects = personalProjects;
        Certifications = certifications;
        Publications = publications;
        Awards = awards;
    }

    public void Edit(Name? name, string? email, string? phone, Address? address, string? summary, string[] skills,
        string[] languages, Experience[] experiences, Education[] educations, PersonalProject[] personalProjects,
        Certification[] certifications, Publication[] publications, Award[] awards) {
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        Summary = summary;
        Skills = skills;
        Languages = languages;
        Experiences = experiences;
        Educations = educations;
        PersonalProjects = personalProjects;
        Certifications = certifications;
        Publications = publications;
        Awards = awards;
    }
}
