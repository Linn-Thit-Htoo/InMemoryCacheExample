using InMemoryCacheExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCacheExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly AppDbContext _appDbContext;

        public BlogController(IMemoryCache cache, AppDbContext appDbContext)
        {
            _cache = cache;
            _appDbContext = appDbContext;
        }

        #region Get Blogs
        [HttpGet]
        public async Task<IActionResult> GetBlogs()
        {
            try
            {
                List<BlogDataModel>? blogs = new();
                if (_cache.TryGetValue("blogs", out blogs))
                {
                    return Ok(blogs);
                }

                blogs = await _appDbContext.Blogs
                    .AsNoTracking()
                    .OrderByDescending(x => x.Blog_Id)
                    .ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                };

                _cache.Set("blogs", blogs, cacheOptions);

                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
