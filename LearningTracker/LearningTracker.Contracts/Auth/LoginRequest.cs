using System.ComponentModel.DataAnnotations;

namespace LearningTracker.Contracts.Auth;

public record LoginRequest([Required] string Login, [Required] string Password);
