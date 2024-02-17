using DemoApi.Configurations;
using DemoApi.Services;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var expirationTime = TimeSpan.FromDays(2); // Default expiration time
var redisCacheSettings = builder.Configuration.GetSection("RedisCache").Get<RedisCacheSettings>();
if(redisCacheSettings == null) throw new ArgumentNullException(nameof(redisCacheSettings));

expirationTime = redisCacheSettings.ExpirationTime;
var redisConnectionString = redisCacheSettings.ConnectionStrings; // builder.Configuration["RedisCache:ConnectionStrings"];
if(string.IsNullOrEmpty(redisConnectionString)) throw new ArgumentNullException(nameof(redisConnectionString));

builder.Services.AddStackExchangeRedisCache(o =>
{
    o.Configuration = redisConnectionString;
    o.InstanceName = "DemoApi-";
});

builder.Services.AddScoped<ICacheService>(provider =>
{
    var distributedCache = provider.GetRequiredService<IDistributedCache>();
    return new CacheService(distributedCache, expirationTime, redisConnectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
