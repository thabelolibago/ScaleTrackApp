using System.Security.Claims;

namespace ScaleTrackAPI.Shared.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var idValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idValue, out var id) ? id : null;
        }

        public static string? GetEmail(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.Email);

        public static string? GetRole(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.Role);
    }
}
