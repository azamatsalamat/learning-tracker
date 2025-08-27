using System.ComponentModel.DataAnnotations;

namespace LearningTracker.Contracts.Auth;

public record RegisterUserRequest([Required] string Login, [Required] string Password);