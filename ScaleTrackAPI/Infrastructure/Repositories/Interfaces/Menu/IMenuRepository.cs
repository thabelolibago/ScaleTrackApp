using ScaleTrackAPI.Domain.Entities;
using System.Security.Claims;

namespace ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IMenuRepository
{
    public interface IMenuRepository
    {
        Task<List<MenuItem>> GetMenuItemsForUserAsync(ClaimsPrincipal user);
    }
}
