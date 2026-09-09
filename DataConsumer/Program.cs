

using DataConsumer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", false)
    .Build();

var service = new ServiceCollection();

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
