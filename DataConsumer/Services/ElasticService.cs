using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataConsumer.Models;
using Elastic.Clients.Elasticsearch;

namespace DataConsumer.Services
{
    public class ElasticService
    {
        private readonly ElasticsearchClient _client;
        public ElasticService(
            ServicesConfigService services
        )
        {
            _client = services.ElasticBuild();
        }

        public async Task<bool> CreateIndexWithMapping()
        {
            var indexName = "reports";

            var existIndex = await _client.Indices.ExistsAsync(indexName);

            if (!existIndex.IsValidResponse)
            {
                return false;
            }

            if (existIndex.Exists)
            {
                System.Console.WriteLine("Mapping index already exist");
                return true;
            }

            var response = await _client.Indices.CreateAsync<Report>(c => c
                .Index(indexName)
                .Mappings(m => m
                    .Properties(p => p
                        .Keyword(k => k.reportId)
                        .Date(k => k.timestamp)
                        .Keyword(k => k.agentId)
                        .Keyword(k => k.unit)
                        .Keyword(k => k.theater)
                        .Keyword(k => k.sector)
                        .Keyword(k => k.location)
                        .Keyword(k => k.reportType)
                        .Keyword(k => k.priority)
                        .Keyword(k => k.sourceType)
                        .Text(k => k.message)
                        .Keyword(k => k.subjectId!)
                        .Keyword(k => k.subjectType!)
                        )));

            return response.IsValidResponse;
        }

        public async Task<bool> IndexDoc(Report report)
        {
            var response = await _client.IndexAsync(report, i => i
                .Index("reports")
                .Id(report.reportId));

            return response.IsValidResponse;
        }
    }
}