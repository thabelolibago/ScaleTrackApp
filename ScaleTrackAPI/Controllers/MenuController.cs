using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.Services;

namespace ScaleTrackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController(MenuService menuService) : ControllerBase
    {
        private readonly MenuService _menuService = menuService;

        [HttpGet("items")]
        [Authorize]
        public async Task<IActionResult> GetMenu()
        {
            var menuItems = await _menuService.GetMenuForUserAsync(User);

            if (!menuItems.Any())
                return Unauthorized("No role assigned to this user.");

            return Ok(menuItems);
        }
    }
}
