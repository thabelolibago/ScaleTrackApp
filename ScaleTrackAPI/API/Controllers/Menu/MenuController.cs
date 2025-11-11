using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.Application.Features.Menu.Services.MenuService;

namespace ScaleTrackAPI.Controllers.Menu.MenuController
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
            return Ok(menuItems);
        }
    }
}
