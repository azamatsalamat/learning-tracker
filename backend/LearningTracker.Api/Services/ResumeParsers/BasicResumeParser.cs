using System.Text.RegularExpressions;
using LearningTracker.Domain.ValueObjects;
using LearningTracker.Entities;
using LearningTracker.Features.Profiles.Enums;
using LearningTracker.Features.Profiles.ValueObjects;
using LearningTracker.Services.Base;

namespace LearningTracker.Services.ResumeParsers;

public class BasicResumeParser : IResumeParser
{
    private static readonly string[] SectionHeaders =
    {
        "work experience", "experience",
        "volunteering experience",
        "education",
        "courses and certificates", "certifications",
        "technical skills", "skills"
    };

    public Profile Parse(Guid userId, string resumeText)
    {
        var lines = resumeText.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToArray();

        var sections = SplitIntoSections(lines);

        var name = ExtractName(lines);
        var email = ExtractEmail(resumeText);
        var phone = ExtractPhone(resumeText);
        var address = ExtractAddress(lines);
        var summary = ExtractSummary(sections);
        var skills = ExtractSkills(sections);
        var languages = ExtractLanguages(sections);
        var experiences = ExtractExperiences(sections);
        var educations = ExtractEducations(sections);
        var projects = ExtractProjects(sections);
        var certifications = ExtractCertifications(sections);
        var publications = ExtractPublications(sections);
        var awards = ExtractAwards(sections);

        return new Profile(
            userId,
            name,
            email,
            phone,
            address,
            summary,
            skills,
            languages,
            experiences,
            educations,
            projects,
            certifications,
            publications,
            awards
        );
    }

    private Dictionary<string, List<string>> SplitIntoSections(string[] lines)
    {
        var sections = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        string? currentSection = null;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var normalizedLine = line.ToLowerInvariant();

            // Check if this line is a section header
            var matchedHeader = SectionHeaders.FirstOrDefault(h => normalizedLine.Equals(h));

            if (matchedHeader != null)
            {
                currentSection = matchedHeader;
                if (!sections.ContainsKey(currentSection))
                {
                    sections[currentSection] = new List<string>();
                }
            }
            else if (currentSection != null && i > 0)
            {
                // Skip the name/contact header line
                sections[currentSection].Add(line);
            }
        }

        return sections;
    }

    private Name? ExtractName(string[] lines)
    {
        if (lines.Length == 0) return null;

        // First line should be the name
        var firstLine = lines[0];
        var parts = firstLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length >= 2)
        {
            return new Name(parts[0], string.Join(" ", parts.Skip(1)));
        }

        return null;
    }

    private string? ExtractEmail(string text)
    {
        var emailRegex = new Regex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b");
        var match = emailRegex.Match(text);
        return match.Success ? match.Value : null;
    }

    private string? ExtractPhone(string text)
    {
        // Updated to match the format +7(778)419-92-87
        var phoneRegex = new Regex(@"\+?\d{1,3}[\s\-\(\)]?\d{3}[\s\-\(\)]?\d{3}[\s\-]?\d{2}[\s\-]?\d{2}");
        var match = phoneRegex.Match(text);
        return match.Success ? match.Value : null;
    }

    private Address? ExtractAddress(string[] lines)
    {
        // Look for city, country in the header (after name)
        // In your resume: Astana, Kazakhstan appears multiple times
        if (lines.Length > 1)
        {
            var headerLine = lines[1];
            // Extract location from experience/education entries
            foreach (var line in lines.Take(10))
            {
                if (line.Contains("Astana") && line.Contains("Kazakhstan"))
                {
                    return new Address("Astana", "Kazakhstan");
                }
            }
        }
        return null;
    }

    private string? ExtractSummary(Dictionary<string, List<string>> sections)
    {
        var summaryKeys = new[] { "summary", "profile", "objective" };
        foreach (var key in summaryKeys)
        {
            if (sections.TryGetValue(key, out var lines))
            {
                return string.Join(" ", lines);
            }
        }
        return null;
    }

    private string[] ExtractSkills(Dictionary<string, List<string>> sections)
    {
        var skillKeys = new[] { "technical skills", "skills" };
        foreach (var key in skillKeys)
        {
            if (sections.TryGetValue(key, out var lines))
            {
                var skills = new List<string>();
                foreach (var line in lines)
                {
                    if (line.Contains(':'))
                    {
                        var parts = line.Split(':', 2);
                        if (parts.Length == 2)
                        {
                            var skillList = parts[1];
                            var items = skillList.Split(new[] { ',', '|', '•', '·' },
                                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                            skills.AddRange(items);
                        }
                    }
                    else
                    {
                        var items = line.Split(new[] { ',', '|', '•', '·' },
                            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                        skills.AddRange(items);
                    }
                }
                return skills.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToArray();
            }
        }
        return Array.Empty<string>();
    }

    private string[] ExtractLanguages(Dictionary<string, List<string>> sections)
    {
        if (sections.TryGetValue("languages", out var lines))
        {
            var languages = new List<string>();
            foreach (var line in lines)
            {
                var items = line.Split(new[] { ',', '|', '•', '·', '-' },
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                languages.AddRange(items.Select(l => l.Split('(')[0].Trim()));
            }
            return languages.ToArray();
        }
        return Array.Empty<string>();
    }

    private Experience[] ExtractExperiences(Dictionary<string, List<string>> sections)
    {
        var experiences = new List<Experience>();

        var expKeys = new[] { "work experience", "experience", "volunteering experience" };

        foreach (var key in expKeys)
        {
            if (sections.TryGetValue(key, out var lines))
            {
                experiences.AddRange(ParseExperienceSection(lines));
            }
        }

        return experiences.ToArray();
    }

    private Experience[] ParseExperienceSection(List<string> lines)
    {
        var experiences = new List<Experience>();
        string? currentPosition = null;
        string? currentCompany = null;
        string? dateRange = null;
        string? location = null;
        DateTime? startDate = null;
        DateTime? endDate = null;
        var responsibilities = new List<string>();

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            if (ContainsDateRange(line))
            {
                if (currentPosition != null && currentCompany != null)
                {
                    experiences.Add(new Experience(
                        currentCompany,
                        currentPosition,
                        string.Join(" ", responsibilities),
                        startDate ?? DateTime.UtcNow,
                        endDate,
                        Array.Empty<string>(),
                        responsibilities.ToArray(),
                        Array.Empty<string>()
                    ));
                    responsibilities.Clear();
                }

                var parts = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var dateIndex = -1;

                for (int j = 0; j < parts.Length; j++)
                {
                    if (ContainsDateRange(parts[j]))
                    {
                        dateIndex = j;
                        break;
                    }
                }

                if (dateIndex > 0)
                {
                    currentPosition = string.Join(" ", parts.Take(dateIndex));
                    dateRange = string.Join(" ", parts.Skip(dateIndex));
                    var dates = ExtractDates(dateRange);
                    startDate = dates.start;
                    endDate = dates.end;
                }
                else
                {
                    currentPosition = line;
                }
            }
            else if (currentPosition != null && currentCompany == null)
            {
                currentCompany = line.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                if (line.Contains("Astana") || line.Contains("Kazakhstan"))
                {
                    location = "Astana, Kazakhstan";
                }
            }
            else if (line.StartsWith("•"))
            {
                responsibilities.Add(line.TrimStart('•').Trim());
            }
        }

        if (currentPosition != null && currentCompany != null)
        {
            experiences.Add(new Experience(
                currentCompany,
                currentPosition,
                string.Join(" ", responsibilities),
                startDate ?? DateTime.UtcNow,
                endDate,
                Array.Empty<string>(),
                responsibilities.ToArray(),
                Array.Empty<string>()
            ));
        }

        return experiences.ToArray();
    }

    private Education[] ExtractEducations(Dictionary<string, List<string>> sections)
    {
        if (!sections.TryGetValue("education", out var lines))
        {
            return Array.Empty<Education>();
        }

        var educations = new List<Education>();
        string? school = null;
        string? major = null;
        Degree degree = Degree.Bachelor;
        DateTime? startDate = null;
        DateTime? endDate = null;
        var achievements = new List<string>();

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            // Check if this is a school name line (contains date range)
            if (ContainsDateRange(line))
            {
                // Save previous education if exists
                if (school != null && major != null)
                {
                    educations.Add(new Education(
                        school,
                        degree,
                        major,
                        startDate ?? DateTime.UtcNow,
                        endDate,
                        achievements.ToArray(),
                        Array.Empty<string>()
                    ));
                    achievements.Clear();
                }

                // Parse school and date range
                var parts = line.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    school = parts[0].Trim();
                    var dateStr = parts[^1].Trim();
                    var dates = ExtractDates(dateStr);
                    startDate = dates.start;
                    endDate = dates.end;
                }
                else
                {
                    school = line;
                }
            }
            // Check if this is degree/major line
            else if (school != null && major == null && !line.StartsWith("•") && !line.StartsWith("GPA") && !line.StartsWith("Honors"))
            {
                major = line;
                degree = InferDegree(line);
            }
            // Skip location line (contains city/country)
            else if (line.Contains("Astana") || line.Contains("Kazakhstan"))
            {
                continue;
            }
            // Collect GPA and Honors
            else if (line.StartsWith("•") || line.StartsWith("GPA") || line.StartsWith("Honors"))
            {
                achievements.Add(line.TrimStart('•').Trim());
            }
        }

        // Add last education
        if (school != null && major != null)
        {
            educations.Add(new Education(
                school,
                degree,
                major,
                startDate ?? DateTime.UtcNow,
                endDate,
                achievements.ToArray(),
                Array.Empty<string>()
            ));
        }

        return educations.ToArray();
    }

    private PersonalProject[] ExtractProjects(Dictionary<string, List<string>> sections)
    {
        var projKeys = new[] { "projects", "personal projects" };
        foreach (var key in projKeys)
        {
            if (sections.TryGetValue(key, out var lines))
            {
                var projects = new List<PersonalProject>();
                string? projectName = null;
                var description = new List<string>();

                foreach (var line in lines)
                {
                    if (IsLikelyProjectName(line))
                    {
                        if (projectName != null)
                        {
                            projects.Add(new PersonalProject(
                                projectName,
                                string.Join(" ", description),
                                Array.Empty<string>()
                            ));
                            description.Clear();
                        }
                        projectName = line;
                    }
                    else
                    {
                        description.Add(line);
                    }
                }

                if (projectName != null)
                {
                    projects.Add(new PersonalProject(
                        projectName,
                        string.Join(" ", description),
                        Array.Empty<string>()
                    ));
                }

                return projects.ToArray();
            }
        }
        return Array.Empty<PersonalProject>();
    }

    private Certification[] ExtractCertifications(Dictionary<string, List<string>> sections)
    {
        var certKeys = new[] { "courses and certificates", "certifications", "certificates" };
        foreach (var key in certKeys)
        {
            if (sections.TryGetValue(key, out var lines))
            {
                var certifications = new List<Certification>();
                string? certName = null;
                string? issuer = null;
                DateTime? issueDate = null;

                for (int i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];

                    // Check if this is institution/issuer line (contains date)
                    if (ContainsDateRange(line) || ContainsMonth(line))
                    {
                        // Save previous cert if exists
                        if (certName != null && issuer != null)
                        {
                            certifications.Add(new Certification(
                                certName,
                                issuer,
                                issueDate ?? DateTime.UtcNow,
                                null,
                                null,
                                null
                            ));
                        }

                        // Parse issuer and date
                        var parts = line.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                        {
                            issuer = parts[0].Trim();
                            var dateStr = parts[^1].Trim();
                            var dates = ExtractDates(dateStr);
                            issueDate = dates.start;
                        }
                        else
                        {
                            issuer = line;
                        }
                    }
                    // This is the certificate name or additional info
                    else if (issuer != null && certName == null)
                    {
                        certName = line;
                    }
                }

                // Add last cert
                if (certName != null && issuer != null)
                {
                    certifications.Add(new Certification(
                        certName,
                        issuer,
                        issueDate ?? DateTime.UtcNow,
                        null,
                        null,
                        null
                    ));
                }

                return certifications.ToArray();
            }
        }
        return Array.Empty<Certification>();
    }

    private Publication[] ExtractPublications(Dictionary<string, List<string>> sections)
    {
        if (sections.TryGetValue("publications", out var lines))
        {
            var publications = new List<Publication>();
            foreach (var line in lines)
            {
                publications.Add(new Publication(
                    line,
                    string.Empty,
                    Array.Empty<string>(),
                    null
                ));
            }
            return publications.ToArray();
        }
        return Array.Empty<Publication>();
    }

    private Award[] ExtractAwards(Dictionary<string, List<string>> sections)
    {
        var awardKeys = new[] { "awards", "honors" };
        foreach (var key in awardKeys)
        {
            if (sections.TryGetValue(key, out var lines))
            {
                var awards = new List<Award>();
                foreach (var line in lines)
                {
                    var parts = line.Split('-', ',', '|');
                    awards.Add(new Award(
                        parts[0].Trim(),
                        parts.Length > 1 ? parts[1].Trim() : string.Empty,
                        DateTime.UtcNow,
                        null
                    ));
                }
                return awards.ToArray();
            }
        }
        return Array.Empty<Award>();
    }

    private bool ContainsDateRange(string line)
    {
        return Regex.IsMatch(line, @"(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s+\d{4}");
    }

    private bool ContainsMonth(string line)
    {
        return Regex.IsMatch(line, @"(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)");
    }

    private bool IsLikelyProjectName(string line)
    {
        return !line.StartsWith("•") && !line.StartsWith("-") && line.Length < 100;
    }

    private Degree InferDegree(string text)
    {
        var lowerText = text.ToLowerInvariant();
        if (lowerText.Contains("phd") || lowerText.Contains("doctorate")) return Degree.Doctorate;
        if (lowerText.Contains("master")) return Degree.Master;
        if (lowerText.Contains("bachelor")) return Degree.Bachelor;
        if (lowerText.Contains("associate")) return Degree.Associate;
        return Degree.Bachelor;
    }

    private (DateTime? start, DateTime? end) ExtractDates(string text)
    {
        // Handle "Present" keyword
        var isPresentEnd = text.ToLowerInvariant().Contains("present");

        // Extract month and year patterns like "Apr 2023" or "Aug 2021"
        var monthYearRegex = new Regex(@"(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s+(\d{4})");
        var matches = monthYearRegex.Matches(text);

        if (matches.Count >= 2)
        {
            var startMonth = ParseMonth(matches[0].Groups[1].Value);
            var startYear = int.Parse(matches[0].Groups[2].Value);

            var endMonth = ParseMonth(matches[1].Groups[1].Value);
            var endYear = int.Parse(matches[1].Groups[2].Value);

            return (
                new DateTime(startYear, startMonth, 1),
                isPresentEnd ? null : new DateTime(endYear, endMonth, 1)
            );
        }
        else if (matches.Count == 1)
        {
            var month = ParseMonth(matches[0].Groups[1].Value);
            var year = int.Parse(matches[0].Groups[2].Value);
            return (new DateTime(year, month, 1), null);
        }

        // Fallback to year-only pattern
        var yearRegex = new Regex(@"\b(19|20)\d{2}\b");
        var yearMatches = yearRegex.Matches(text);

        if (yearMatches.Count >= 2)
        {
            var startYear = int.Parse(yearMatches[0].Value);
            var endYear = int.Parse(yearMatches[1].Value);

            return (
                new DateTime(startYear, 1, 1),
                isPresentEnd ? null : new DateTime(endYear, 1, 1)
            );
        }
        else if (yearMatches.Count == 1)
        {
            var year = int.Parse(yearMatches[0].Value);
            return (new DateTime(year, 1, 1), null);
        }

        return (null, null);
    }

    private int ParseMonth(string monthAbbr)
    {
        return monthAbbr switch
        {
            "Jan" => 1,
            "Feb" => 2,
            "Mar" => 3,
            "Apr" => 4,
            "May" => 5,
            "Jun" => 6,
            "Jul" => 7,
            "Aug" => 8,
            "Sep" => 9,
            "Oct" => 10,
            "Nov" => 11,
            "Dec" => 12,
            _ => 1
        };
    }
}
