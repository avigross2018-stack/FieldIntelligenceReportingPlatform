using DataConsumer.Services;
using Elastic.Esql.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", false)
    .Build();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/myapp-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var service = new ServiceCollection();

service.AddLogging(log =>
{
    log.ClearProviders();
    log.AddSerilog(Log.Logger);
});
service.AddSingleton<IConfiguration>(configuration);
service.AddSingleton<MainSystemService>();
service.AddSingleton<ServicesConfigService>();
service.AddSingleton<ElasticService>();
service.AddSingleton<ValidatorService>();


var serviceProvider = service.BuildServiceProvider();
var consumer = serviceProvider.GetRequiredService<MainSystemService>();

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    await consumer.RunAsync(cts.Token, configuration["Kafka:RawTopic"]);
}
catch(Exception ex)
{
    System.Console.WriteLine($"ERROR: {ex.Message}");
}
