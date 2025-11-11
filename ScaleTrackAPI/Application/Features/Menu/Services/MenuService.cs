using System.Security.Claims;
using ScaleTrackAPI.Application.Features.Menu.DTOs;
using ScaleTrackAPI.Application.Features.Menu.Mappers.MenuItemMapper;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IMenuRepository;

namespace ScaleTrackAPI.Application.Features.Menu.Services.MenuService
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
