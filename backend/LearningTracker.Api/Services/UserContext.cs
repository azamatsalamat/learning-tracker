using System.Security.Claims;
using LearningTracker.Services.Base;

namespace LearningTracker.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _contextAccessor;

    public UserContext(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public Guid UserId {
        get {
            var claim = _contextAccessor.HttpContext!.User.Claims.Single(x => x.Type == ClaimsIdentity.DefaultNameClaimType);
            return Guid.Parse(claim.Value);
        }
    }
}
