using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IMenuRepository;
using System.Security.Claims;

namespace ScaleTrackAPI.Infrastructure.Repositories.Implementations.Menu
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

            var allMenuItems = await _context.MenuItems
                .Where(m => !string.IsNullOrWhiteSpace(m.Roles))
                .ToListAsync();

            var filteredMenuItems = allMenuItems
                .Where(m =>
                    m.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Any(r => userRoles.Contains(r.Trim())))
                .ToList();

            return filteredMenuItems;
        }
    }
}

