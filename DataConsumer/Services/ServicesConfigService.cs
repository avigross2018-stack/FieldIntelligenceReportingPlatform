using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;

namespace DataConsumer.Services
{
    public class ServicesConfigService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ElasticsearchClient _elasticsearchClient;
        public ServicesConfigService(
            IConfiguration config
        )
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServer"],
                GroupId = config["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

            var ElasticConfig = new ElasticsearchClientSettings(
                new Uri(config["ElasticSearch:Uri"])
            );

            _elasticsearchClient = new ElasticsearchClient(ElasticConfig);
        }

        public IConsumer<Ignore, string> ConsumerBuild()
        {
            return _consumer;
        }

        public ElasticsearchClient ElasticBuild()
        {
            return _elasticsearchClient;
        }
        
    }
}