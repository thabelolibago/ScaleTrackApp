using ScaleTrackAPI.DTOs.Menu;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Repositories;
using System.Security.Claims;

namespace ScaleTrackAPI.Services
{
    public class MenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<List<MenuItemResponse>> GetMenuForUserAsync(ClaimsPrincipal user)
        {
            var items = await _menuRepository.GetMenuItemsForUserAsync(user);
            return items.ToResponseList();
        }
    }
}
