using Elastic.Clients.Elasticsearch;
using ElasticApi.Repos;
using ElasticApi.Services;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", false)
    .Build();

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