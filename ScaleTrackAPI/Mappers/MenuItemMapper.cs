using ScaleTrackAPI.DTOs.Menu;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Helpers
{
    public static class MenuItemMapper
    {
        public static MenuItemResponse ToResponse(MenuItem menuItem)
        {
            if (menuItem == null) return null!;

            return new MenuItemResponse
            {
                Name = menuItem.Name,
                Path = menuItem.Path,
                Icon = menuItem.Icon
            };
        }

        public static List<MenuItemResponse> ToResponseList(IEnumerable<MenuItem> menuItems)
        {
            return menuItems.Select(ToResponse).ToList();
        }
    }
}
