using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Database;
using ScaleTrackAPI.Models;
using System.Security.Claims;

namespace ScaleTrackAPI.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly AppDbContext _context;

        public MenuRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItem>> GetMenuItemsForUserAsync(ClaimsPrincipal user)
        {
            if (user == null) return new List<MenuItem>();

            var userRoles = user.Claims
                                .Where(c => c.Type == ClaimTypes.Role)
                                .Select(c => c.Value.Trim())
                                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!userRoles.Any()) return new List<MenuItem>();

            return await _context.MenuItems
                .Where(m => !string.IsNullOrWhiteSpace(m.Roles) &&
                            m.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Any(r => userRoles.Contains(r.Trim())))
                .ToListAsync();
        }
    }
}
