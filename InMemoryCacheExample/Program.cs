using InMemoryCacheExample.Data;
using InMemoryCacheExample.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
}, ServiceLifetime.Transient);

builder.Services.AddMemoryCache();
builder.Services.Configure<MemoryCacheEntryOptions>(options =>
{
    options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
});

builder.Services.AddSingleton<CacheService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
