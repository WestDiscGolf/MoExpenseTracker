namespace MoExpenseTracker.Core;

public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public int UserId()
    {
        if (!(accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false))
        {
            // throw new AppException("Unauthorized", 401);
            throw new UnauthorizedAccessException();
        }

        var userIdClaim = accessor.HttpContext.User.FindFirst("id")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException();
        }

        if (!int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException();
        }

        return userId;
    }

}

public interface ICurrentUser
{
    int UserId();
}
