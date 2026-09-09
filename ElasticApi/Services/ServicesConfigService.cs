using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;

namespace ElasticApi.Services
{
    public class ServicesConfigService
    {
        private readonly ElasticsearchClient _client;
        public ServicesConfigService(
            IConfiguration configuration
        )
        {
            var elasticConfig = new ElasticsearchClientSettings(
                new Uri(configuration["Elastic:Uri"])
            );

            _client = new ElasticsearchClient(elasticConfig);
        }

        public ElasticsearchClient ElasticConfig()
        {
            return _client;
        }
    }
}