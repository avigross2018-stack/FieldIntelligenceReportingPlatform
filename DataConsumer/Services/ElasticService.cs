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

            var response = await _client.Indices.CreateAsync<ReportElastic>(c => c
                .Index(indexName)
                .Mappings(m => m
                    .Properties(p => p
                        .Keyword(k => k.ReportId)
                        .Date(k => k.Timestamp)
                        .Keyword(k => k.AgentId)
                        .Keyword(k => k.Unit)
                        .Keyword(k => k.Theater)
                        .Keyword(k => k.Sector)
                        .Keyword(k => k.Location)
                        .Keyword(k => k.ReportType)
                        .Keyword(k => k.Priority)
                        .Keyword(k => k.SourceType)
                        .Text(k => k.Message)
                        .Keyword(k => k.SubjectId!)
                        .Keyword(k => k.SubjectType!)
                        )));

            return response.IsValidResponse;
        }

        public async Task<bool> IndexDoc(ReportElastic report)
        {
            var response = await _client.IndexAsync(report, i => i
                .Index("reports")
                .Id(report.ReportId).OpType(OpType.Create));

            return response.IsValidResponse;
        }
    }
}