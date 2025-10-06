namespace LearningTracker.Configurations;

public class AuthOptions
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public TimeSpan AccessTokenLifetime { get; set; }
}
