using WebsitePerformanceEvaluator.API;
using WebsitePerformanceEvaluator.Core;
using WebsitePerformanceEvaluator.Data;
using WebsitePerformanceEvaluator.Web.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.ConfigureAPIServices();
builder.Services.ConfigureDataServices(builder.Configuration);
builder.Services.ConfigureCoreServices();
builder.Services.ConfigureWebCoreServices();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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