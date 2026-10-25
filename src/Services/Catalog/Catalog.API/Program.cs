using Catalog.API.Data;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config => 
{
    config.RegisterServicesFromAssembly(assembly);
    //با این کار، ValidationBehavior به Pipeline مربوط به MediatR اضافه می‌شود
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
    //ثبت هر دو Behavior باعث می‌شود در خط لوله درخواست‌های Mediator قرار بگیرند. ترتیب ثبت آن‌ها نیز ترتیب اجرای رفتارها را تعیین می‌کند.
});

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddCarter();

builder.Services.AddMarten(options =>
{
    options.Connection(
        builder.Configuration.GetConnectionString("CatalogDb")!);

}).UseLightweightSessions();

if (builder.Environment.IsDevelopment())
{
    builder.Services.InitializeMartenWith<CatalogInitialData>();
}
//این دستور کلاس CustomExceptionHandler را به سیستم Dependency Injection داخلی ASP.NET Core معرفی می‌کند
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services
    .AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("CatalogDb")!);
var app = builder.Build();

//این دستور مدیریت استثنا را در Pipeline درخواست‌های HTTP فعال می‌کند و از Handler ثبت‌شده استفاده می‌کند.
app.UseExceptionHandler(options => { });
app.MapCarter();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
