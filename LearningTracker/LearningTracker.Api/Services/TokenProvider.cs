using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LearningTracker.Api.Configurations;
using LearningTracker.Api.Services.Base;
using LearningTracker.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LearningTracker.Api.Services;

public class TokenProvider : ITokenProvider
{
    private readonly AuthOptions _authOptions;

    public TokenProvider(IOptions<AuthOptions> authOptions)
    {
        _authOptions = authOptions.Value;
    }

    public Task<string> GenerateAccessToken(User user, CancellationToken ct)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.Key));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokeOptions = new JwtSecurityToken(
            issuer: _authOptions.Issuer,
            audience: _authOptions.Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Login)
            ],
            expires: DateTime.UtcNow.Add(_authOptions.AccessTokenLifetime),
            signingCredentials: signinCredentials,
            notBefore:DateTime.UtcNow.Subtract(_authOptions.AccessTokenLifetime)
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return Task.FromResult(tokenString);
    }
}
