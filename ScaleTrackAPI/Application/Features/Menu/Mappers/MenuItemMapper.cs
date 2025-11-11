
using ScaleTrackAPI.Application.Features.Menu.DTOs;
using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.Menu.Mappers.MenuItemMapper
{
    public static class MenuItemMapper
    {
        public static MenuItemResponse ToResponse(this MenuItem menuItem)
        {
            if (menuItem == null) 
                throw new ArgumentNullException(nameof(menuItem));

            return new MenuItemResponse
            {
                Name = menuItem.Name,
                Path = menuItem.Path,
                Icon = menuItem.Icon
            };
        }

        public static List<MenuItemResponse> ToResponseList(this IEnumerable<MenuItem> menuItems)
        {
            return menuItems?.Select(i => i.ToResponse()).ToList() ?? new List<MenuItemResponse>();
        }

        public static MenuItemDto ToDto(this MenuItem menuItem)
        {
            return new MenuItemDto
            {
                Name = menuItem.Name,
                Path = menuItem.Path,
                Icon = menuItem.Icon,
                Roles = menuItem.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            };
        }
    }
}
