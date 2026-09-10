using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using DataConsumer.Exceptions;
using DataConsumer.Models;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;

namespace DataConsumer.Services
{
    public class MainSystemService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ElasticService _elasticsearch;
        private readonly ValidatorService _validator;
        private readonly ILogger<MainSystemService> _logger;
        public MainSystemService(
            ServicesConfigService services,
            ElasticService elasticService,
            ValidatorService validator,
            ILogger<MainSystemService> logger
        )
        {
            _consumer = services.ConsumerBuild();
            _elasticsearch = elasticService;
            _validator = validator;
            _logger = logger;
        }

        public async Task RunAsync(
            CancellationToken cancellationToken,
            string topicName
            )
        {
            var createIndexSuccess = await _elasticsearch.CreateIndexWithMapping();
            if (!createIndexSuccess)
            {
                _logger.LogError("Failed to create new index");
                throw new FailedCreatingIndex("Failed to create new Index");
            }

            _consumer.Subscribe(topicName);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var msg = _consumer.Consume(TimeSpan.FromSeconds(1));
                    if(msg == null || msg.Message?.Value == null)
                    {
                        System.Console.WriteLine("Failed to consume press Ctrl + C to stop ...");
                        continue;
                    }
                    _logger.LogInformation("Consume message from Kafka successfully");
                    
                    var model = JsonSerializer.Deserialize<ReportJson>(msg.Message.Value,
                        new JsonSerializerOptions{ PropertyNameCaseInsensitive= true });
                    var elasticModel = new ReportElastic
                    {
                        ReportId = model.ReportId,
                        Timestamp = model.Timestamp,
                        AgentId = model.AgentId,
                        Unit = model.Unit,
                        Theater = model.Theater,
                        Sector = model.Sector,
                        Location = model.Location,
                        ReportType = model.ReportType,
                        Priority = model.Priority,
                        SourceType = model.SourceType,
                        Message = model.Message,
                        SubjectId = model.SubjectId,
                        SubjectType = model.SubjectType,
                        ProcessAt = DateTime.UtcNow
                    };
                    if (!_validator.ValidateData(elasticModel))
                    {
                        // System.Console.WriteLine("Data is not Valid");
                        _logger.LogWarning("Model from Kafka is not valid ID:{id}", elasticModel.ReportId);
                        continue;
                    }

                    var successPutDoc = await _elasticsearch.IndexDoc(elasticModel);
                    if (!successPutDoc)
                    {
                        // System.Console.WriteLine(("Failed to PUT Doc"));
                        _logger.LogWarning("Failed to upload doc to Elastic ID:{id}", elasticModel.ReportId);
                        continue;
                    }

                    // System.Console.WriteLine("Upload doc to Elastic successfully");
                    _logger.LogInformation("Upload doc to Elastic successfully ID:{id}", elasticModel.ReportId);
                    _consumer.Commit(msg);
                }
                catch
                {
                    _logger.LogWarning("Massage skipped...");
                    continue;
                }
                
            }
        }
    }
}