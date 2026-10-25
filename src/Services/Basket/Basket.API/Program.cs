using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
//این دستور سرویس‌های موردنیاز Carter را در Dependency Injection Container ثبت می‌کند.
builder.Services.AddCarter();

builder.Services.AddMediatR(config =>
{
    //بت سرویس‌های Mediator: با استفاده از RegisterServicesFromAssembly، سرویس‌های موردنیاز Mediator در اسمبلی فعلی ثبت می‌شوند.
    config.RegisterServicesFromAssembly(
        typeof(Program).Assembly);
    //ا AddOpenBehavior، کلاس ValidationBehavior<,> به چرخه پردازش درخواست‌های Mediator اضافه می‌شود.
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    //کلاس LoggingBehavior<,> نیز به همان چرخه اضافه می‌شود.
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

//در نتیجه، رفتارهای اعتبارسنجی و لاگ‌گیری در Request Pipeline مربوط به Mediator قرار می‌گیرند.
builder.Services.AddMarten(options =>
{
    //اتصال به PostgreSQL از طریق Connection String
    options.Connection(
        builder.Configuration.GetConnectionString("Database")!);
    //در این تنظیم، ویژگی Username شناسه هر سند ShoppingCart خواهد بود
    options.Schema.For<ShoppingCart>().Identity(x => x.UserName);
})
    .UseLightweightSessions();//استفاده از Lightweight Session برای بهبود عملکرد

////این ثبت باعث می‌شود هنگام درخواست IBasketRepository، پیاده‌سازی BasketRepository تزریق شود. طول عمر سرویس نیز Scoped است.
//builder.Services.AddScoped<IBasketRepository, BasketRepository>();

//این دستور، CustomExceptionHandler را به‌عنوان سرویس مدیریت استثناها ثبت می‌کند.
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

////در این روش، ثبت سرویس به‌صورت دستی انجام می‌شود:
////سرویس IBasketRepository به‌صورت Scoped ثبت می‌شود
//builder.Services.AddScoped<IBasketRepository>(provider =>
//{
//    //با استفاده از IServiceProvider، پیاده‌سازی اصلی یعنی BasketRepository دریافت می‌شود.
//    var basketRepository = provider.GetRequiredService<IBasketRepository>();
//    var distributedCache = provider.GetRequiredService<IDistributedCache>();
//    //نمونه‌ای از CacheBasketRepository با دریافت مخزن اصلی و IDistributedCache ساخته می‌شود.
//    return new CachedBasketRepository(basketRepository, distributedCache);
//});
//CacheBasketRepository به‌عنوان Decorator روی سرویس ثبت‌شده‌ی IBasketRepository قرار بگیرد
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration.GetConnectionString("Redis");
});
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("Database"))
    .AddRedis(
        builder.Configuration.GetConnectionString("Redis"));

var app = builder.Build();
//این دستور Endpointهای تعریف‌شده با Carter را به برنامه Web API نگاشت می‌کند.
app.MapCarter();

app.UseExceptionHandler(_ => { });
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
