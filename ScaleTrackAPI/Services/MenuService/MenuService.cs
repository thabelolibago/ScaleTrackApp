using ScaleTrackAPI.DTOs.Menu;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Repositories;
using System.Security.Claims;

namespace ScaleTrackAPI.Services
{
    public class MenuService(IMenuRepository menuRepository)
    {
        private readonly IMenuRepository _menuRepository = menuRepository;

        public async Task<List<MenuItemResponse>> GetMenuForUserAsync(ClaimsPrincipal user)
        {
            var items = await _menuRepository.GetMenuItemsForUserAsync(user);
            return MenuItemMapper.ToResponseList(items);
        }
    }
}
