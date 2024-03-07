using InMemoryCacheExample.Data;
using InMemoryCacheExample.Models.Entities;
using InMemoryCacheExample.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InMemoryCacheExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly CacheService _cacheService;

        public UserController(AppDbContext appDbContext, CacheService cacheService)
        {
            _appDbContext = appDbContext;
            _cacheService = cacheService;
        }

        #region Get Users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                //List<UserDataModel>? cachedList = _cacheService.GetData<UserDataModel>("users");
                var cachedList = _cacheService.GetData("users");
                if (cachedList is not null)
                {
                    return Ok(cachedList);
                }

                List<UserDataModel> lst = await _appDbContext.Users
                    .AsNoTracking()
                    .OrderByDescending(x => x.UserId)
                    .ToListAsync();
                _cacheService.SetData("users", lst);

                return Ok(lst);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
