namespace LearningTracker.Features.Profiles.Entities;

public class Certification {
    public string Name { get; protected set; }
    public string Issuer { get; protected set; }
    public DateTime IssueDate { get; protected set; }
    public DateTime? ExpirationDate { get; protected set; }
    public string? CredentialId { get; protected set; }
    public string? CredentialUrl { get; protected set; }

    public Certification(string name, string issuer, DateTime issueDate, DateTime? expirationDate, string? credentialId, string? credentialUrl) {
        Name = name;
        Issuer = issuer;
        IssueDate = issueDate;
        ExpirationDate = expirationDate;
        CredentialId = credentialId;
        CredentialUrl = credentialUrl;
    }
}
