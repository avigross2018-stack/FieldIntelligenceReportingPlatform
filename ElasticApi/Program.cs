using Elastic.Clients.Elasticsearch;
using ElasticApi.Exceptions;
using ElasticApi.Repos;
using ElasticApi.Services;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", false)
    .Build();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/myapp-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddLogging(log =>
{
    log.ClearProviders();
    log.AddSerilog(Log.Logger);
});

builder.Services.AddExceptionHandler<GlobalErrorHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<IConfiguration>(config);
builder.Services.AddSingleton<ServicesConfigService>();

builder.Services.AddSingleton<ElasticsearchClient>(sp => sp
    .GetRequiredService<ServicesConfigService>()
    .ElasticConfig());
builder.Services.AddSingleton<IElasticRepo, ElasticRepo>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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