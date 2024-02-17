using DemoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController(ICacheService cacheService) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Healthy");
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetMySecret(string key)
        {
            var myCacheData = await cacheService.GetCacheDataAsync(key);
            if (myCacheData == null)
            {
                return NotFound();
            }

            return Ok(myCacheData);
        }
    }
}
