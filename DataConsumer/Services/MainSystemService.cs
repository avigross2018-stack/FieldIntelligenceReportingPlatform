using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using DataConsumer.Exceptions;
using DataConsumer.Models;
using Elastic.Clients.Elasticsearch;

namespace DataConsumer.Services
{
    public class MainSystemService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ElasticService _elasticsearch;
        private readonly ValidatorService _validator;
        public MainSystemService(
            ServicesConfigService services,
            ElasticService elasticService,
            ValidatorService validator
        )
        {
            _consumer = services.ConsumerBuild();
            _elasticsearch = elasticService;
            _validator = validator;
        }

        public async Task RunAsync(
            CancellationToken cancellationToken,
            string topicName
            )
        {
            var createIndexSuccess = await _elasticsearch.CreateIndexWithMapping();
            if (!createIndexSuccess)
            {
                throw new FailedCreatingIndex("Failed to create new Index");
            }

            _consumer.Subscribe(topicName);
            while (!cancellationToken.IsCancellationRequested)
            {
                var msg = _consumer.Consume(TimeSpan.FromSeconds(1));
                if(msg == null || msg.Message?.Value == null)
                {
                    System.Console.WriteLine("Failed to consume press Ctrl + C to stop ...");
                    continue;
                }

                var model = JsonSerializer.Deserialize<Report>(msg.Message.Value);
                if (!_validator.ValidateData(model))
                {
                    System.Console.WriteLine("Data is not Valid");
                    continue;
                }

                var successPutDoc = await _elasticsearch.IndexDoc(model);
                if (!successPutDoc)
                {
                    System.Console.WriteLine(("Failed to PUT Doc"));
                    continue;
                }

                System.Console.WriteLine("Upload doc to Elastic successfully");
                _consumer.Commit(msg);
            }
        }
    }
}