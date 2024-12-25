using Microsoft.AspNetCore.Mvc;
using TestTask.Services;

namespace TestTask.API.Controllers;

[ApiController]
[Route("[controller]")]
public class MarketController : ControllerBase
{
    private readonly MarketService _marketService;

    public MarketController(MarketService marketService)
    {
        _marketService = marketService;
    }

    [HttpPost]
    public async Task<IActionResult> BuyAsync(int userId, int itemId)
    {
        // Adding error handling middleware would be so much better...
        try
        {
            await _marketService.BuyAsync(userId, itemId);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest();
        }
    }
}