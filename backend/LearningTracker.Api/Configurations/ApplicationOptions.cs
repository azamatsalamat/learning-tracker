namespace LearningTracker.Configurations;

public class ApplicationOptions
{
    public ApplicationOptions()
    {
        CorsOrigins = Array.Empty<string>();
    }
    public string[] CorsOrigins { get; set; }
}
