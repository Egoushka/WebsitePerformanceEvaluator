using WebsitePerformanceEvaluator.Core;
using WebsitePerformanceEvaluator.Data;
using WebsitePerformanceEvaluator.MVC;
using WebsitePerformanceEvaluator.MVC.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.ConfigureMVCServices();
builder.Services.ConfigureMVCCoreServices();
builder.Services.ConfigureDataServices(builder.Configuration);
builder.Services.ConfigureCoreServices();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(typeof(GlobalExceptionFilter));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Link}/{action=Index}/{id?}");

app.Run();