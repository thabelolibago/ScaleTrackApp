using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Database;
using ScaleTrackAPI.Models;
using System.Security.Claims;

namespace ScaleTrackAPI.Repositories
{
    public class MenuRepository(AppDbContext context) : IMenuRepository
    {
        private readonly AppDbContext _context = context;
        
        public async Task<List<MenuItem>> GetMenuItemsForUserAsync(ClaimsPrincipal user)
        {
            var userRoles = user.Claims
                                .Where(c => c.Type == ClaimTypes.Role)
                                .Select(c => c.Value)
                                .ToList();

            if (!userRoles.Any())
                return new List<MenuItem>();

            var roleSet = userRoles.ToHashSet(StringComparer.OrdinalIgnoreCase);

            var items = await _context.MenuItems.ToListAsync();

            return items
                .Where(i => !string.IsNullOrWhiteSpace(i.Roles) &&
                            i.Roles.Split(',').Any(r => roleSet.Contains(r.Trim())))
                .ToList();
        }
    }
}
