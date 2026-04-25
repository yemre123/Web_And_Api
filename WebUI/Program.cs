using WebUI.Configuration;
using WebUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<FanScheduleUiOptions>(builder.Configuration.GetSection("FanScheduleUi"));
builder.Services.AddHttpClient<FanScheduleApiService>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<FanScheduleUiOptions>>().Value;
    client.BaseAddress = new Uri(options.ApiBaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FanSchedules}/{action=Index}/{id?}");

app.Run();
