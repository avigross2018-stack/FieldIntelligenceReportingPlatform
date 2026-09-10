using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticApi.Exceptions;
using ElasticApi.Models;

namespace ElasticApi.Repos
{
    public class ElasticRepo : IElasticRepo
    {
        private readonly ElasticsearchClient _client;
        private readonly string _indexName;
        public ElasticRepo(
            ElasticsearchClient client,
            IConfiguration config
            )
        {
            _client = client;
            _indexName = config["Elastic:IndexName"];
        }

        public async Task<IEnumerable<Report>> SearchMessage(string text)
        {
            var response = await _client.SearchAsync<Report>(s => s
                .Indices(_indexName)
                .Query(q => q
                    .MatchPhrase(m => m
                        .Field(f => f.Message)
                        .Query(text))));

            if (!response.IsValidResponse)
            {
                Console.WriteLine(response.DebugInformation);
                throw new InvalidElasticInteractionException("Failed to search a message");
            }

            return response.Documents;
        }

        public async Task<IEnumerable<Report>> SearchBySubjectId(string subjectId)
        {
            var response = await _client.SearchAsync<Report>(s => s
                .Indices(_indexName)
                .Query(q => q
                    .Term(t => t
                        .Field(f => f.SubjectId)
                        .Value(subjectId))));

            if (!response.IsValidResponse)
            {
                Console.WriteLine(response.DebugInformation);
                throw new InvalidElasticInteractionException("Failed to search a message");
            }
            return response.Documents;
        }

        public async Task<IEnumerable<Report>> SearchByActivityArea(string? sector, string? theater, string? location)
        {
            var conditions = new List<Action<QueryDescriptor<Report>>>();

            if (!string.IsNullOrWhiteSpace(sector))
            {
                conditions.Add(q => q.Term(t => t.Field(f => f.Sector).Value(sector)));
            }

            if (!string.IsNullOrWhiteSpace(theater))
            {
                conditions.Add(q => q.Term(t => t.Field(f => f.Theater).Value(theater)));
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                conditions.Add(q => q.Term(t => t.Field(f => f.Location).Value(location)));
            }

            var response = await _client.SearchAsync<Report>(s => s
                .Indices(_indexName)
                .Size(1000)
                .Query(q => q.Bool(b => b.Filter(conditions.ToArray()))));
            
            if (!response.IsValidResponse)
            {
                Console.WriteLine(response.DebugInformation);
                throw new InvalidElasticInteractionException("Failed to search By Activity Area");
            }
            return response.Documents;
        }

        public async Task<IEnumerable<Report>> SearchByPriority(
            string? priority, 
            DateTime? from, 
            DateTime? to)
        {
            var conditions = new List<Action<QueryDescriptor<Report>>>();

            if (!string.IsNullOrWhiteSpace(priority))
            {
                var fixCase = char.ToUpper(priority[0]) + priority[1..];
                conditions.Add(q => q.Term(t => t.Field(f => f.Priority).Value(fixCase)));
            }

            if (from.HasValue && from != null)
            {
                conditions.Add(q => q.Range(r => r.Date(d => d.Field(f => f.timestamp).Gte(from))));
            }
            if (to.HasValue && from != null)
            {
                conditions.Add(q => q.Range(r => r.Date(d => d.Field(f => f.timestamp).Lte(to))));
            }

            var response = await _client.SearchAsync<Report>(s => s
                .Indices(_indexName)
                .Size(1000)
                .Query(q => q.Bool(b => b.Filter(conditions.ToArray()))));

            if (!response.IsValidResponse)
            {
                Console.WriteLine(response.DebugInformation);
                throw new InvalidElasticInteractionException("Failed to search By Priority");
            }
            return response.Documents;
        }
    }    
}