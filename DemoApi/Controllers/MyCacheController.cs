using DemoApi.Dtos;
using DemoApi.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyCacheController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        public MyCacheController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        // GET: api/<MyCacheController>
        [HttpGet]
        public async Task<ActionResult<Dictionary<string, string>>> Get()
        {
            // Todo... get all list of key/values from Redis Cache

            var dic = await _cacheService.GetCacheDatasAsync();
            return Ok(dic);
        }

        // GET api/<MyCacheController>/F0ADD831-D13A-4745-9256-F776CE7BE055
        [HttpGet("{key:guid}")]
        public async Task<ActionResult<MyCacheData>> GetAsync(Guid key)
        {
            var myCacheData = await _cacheService.GetCacheDataAsync(key.ToString());

            return Ok(myCacheData);
        }

        // POST api/<MyCacheController>
        [HttpPost("{key:guid}")]
        public async Task<ActionResult> PostAsync(Guid key, [FromBody] MyCacheData cacheData)
        {
            await _cacheService.UpdateCacheDataAsync(key.ToString(), cacheData);

            return Ok();
        }

        // PUT api/<MyCacheController>/5
        [HttpPut("{key:guid}")]
        public async Task<ActionResult> PutAsync(Guid key, [FromBody] MyCacheData cacheData)
        {
            await _cacheService.UpdateCacheDataAsync(key.ToString(), cacheData);

            return Ok();
        }

        // DELETE api/<MyCacheController>/5
        [HttpDelete("{key:guid}")]
        public async Task<ActionResult> Delete(Guid key)
        {
            await _cacheService.DeleteCacheDataAsync(key.ToString());
            return Ok();
        }
    }
}
