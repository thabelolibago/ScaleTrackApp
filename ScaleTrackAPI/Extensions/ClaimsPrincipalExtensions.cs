using System.Security.Claims;

namespace ScaleTrackAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.NameIdentifier);

        public static string? GetEmail(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.Email);
    }
}
