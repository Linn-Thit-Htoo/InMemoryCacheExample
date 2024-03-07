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

                //var cacheOptions = new MemoryCacheEntryOptions
                //{
                //    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                //};
                _cache.Set("blogs", blogs);

                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Update Blog
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlog(int id, UpdateBlogRequestModel requestModel)
        {
            try
            {
                if (IsNullOrEmpty(requestModel.Blog_Title, requestModel.Blog_Author, requestModel.Blog_Content))
                    goto Fail;

                BlogDataModel? item = await _appDbContext.Blogs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Blog_Id == id);
                if (item is null)
                    return NotFound("No data found.");

                _appDbContext.Attach(item);
                item.Blog_Title = requestModel.Blog_Title;
                item.Blog_Author = requestModel.Blog_Author;
                item.Blog_Content = requestModel.Blog_Content;
                int result = await _appDbContext.SaveChangesAsync();

                if (result > 0)
                {
                    List<BlogDataModel> updatedBlogs = await _appDbContext.Blogs
                        .AsNoTracking()
                        .OrderByDescending(x => x.Blog_Id)
                        .ToListAsync();
                    _cache.Set("blogs", updatedBlogs);

                    return StatusCode(202, "Blog data updated.");
                }
                goto Fail;

            Fail:
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        public static bool IsNullOrEmpty(params string[] strings) => strings.Any(string.IsNullOrEmpty);
    }
}
