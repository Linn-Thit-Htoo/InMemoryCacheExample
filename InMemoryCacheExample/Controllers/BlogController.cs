using InMemoryCacheExample.Data;
using InMemoryCacheExample.Models.Entities;
using InMemoryCacheExample.Models.RequestModels;
using InMemoryCacheExample.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InMemoryCacheExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly CacheService _cacheService;

        public BlogController(AppDbContext appDbContext, CacheService cacheService)
        {
            _appDbContext = appDbContext;
            _cacheService = cacheService;
        }

        #region Get Blogs
        [HttpGet]
        public async Task<IActionResult> GetBlogs()
        {
            try
            {
                var cachedList = _cacheService.GetData<BlogDataModel>("blogs");
                if (cachedList is not null)
                {
                    return Ok(cachedList);
                }

                List<BlogDataModel> lst = await _appDbContext.Blogs
                    .AsNoTracking()
                    .OrderByDescending(x => x.Blog_Id)
                    .ToListAsync();

                _cacheService.SetData("blogs", lst);

                return Ok(lst);
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
                    _cacheService.SetData("blogs", updatedBlogs);

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
