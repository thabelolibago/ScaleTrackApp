using ScaleTrackAPI.Models;
using System.Security.Claims;

namespace ScaleTrackAPI.Repositories
{
    public interface IMenuRepository
    {
        Task<List<MenuItem>> GetMenuItemsForUserAsync(ClaimsPrincipal user);
    }
}
