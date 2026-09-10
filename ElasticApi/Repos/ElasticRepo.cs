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
        private readonly ILogger<ElasticRepo> _logger;
        public ElasticRepo(
            ElasticsearchClient client,
            IConfiguration config,
            ILogger<ElasticRepo> logger
            )
        {
            _client = client;
            _indexName = config["Elastic:IndexName"];
            _logger = logger;
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
                // Console.WriteLine(response.DebugInformation);
                _logger.LogWarning("Failed to search a message");
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
                // Console.WriteLine(response.DebugInformation);
                _logger.LogWarning("Failed to search a message");
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
                // Console.WriteLine(response.DebugInformation);
                _logger.LogWarning("Failed to search By Activity Area");
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

            if (from.HasValue)
            {
                conditions.Add(q => q.Range(r => r.Date(d => d.Field(f => f.Timestamp).Gte(from))));
            }
            if (to.HasValue)
            {
                conditions.Add(q => q.Range(r => r.Date(d => d.Field(f => f.Timestamp).Lte(to))));
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

        public async Task<object> GetStatistics()
        {
            var response = await _client.SearchAsync<Report>(s => s
                .Indices(_indexName)
                .Size(1000)
                .Aggregations(a => a
                    .Add("by_priority", ag => ag
                        .Terms(i => i
                            .Field(f => f.Priority)
                            .Size(1000)))
                    .Add("by_report_type", ag => ag
                        .Terms(i => i
                            .Field(f => f.ReportType)
                            .Size(1000)))
                    .Add("by_theater", ag => ag
                        .Terms(i => i
                            .Field(f => f.Theater)
                            .Size(1000)))));

            if (!response.IsValidResponse)
            {
                Console.WriteLine(response.DebugInformation);
                throw new InvalidElasticInteractionException("Failed to get Statistics");
            }

            var priorityAgg = response.Aggregations?.GetStringTerms("by_priority");
            var reportTypeAgg = response.Aggregations?.GetStringTerms("by_report_type");
            var theaterAgg = response.Aggregations?.GetStringTerms("by_theater");

            return new
            {
                ByPriority = priorityAgg?.Buckets.ToDictionary(
                    b => b.Key.ToString(),
                    b => b.DocCount) ?? [],

                ByReportType = reportTypeAgg?.Buckets.ToDictionary(
                    b => b.Key.ToString(),
                    b => b.DocCount
                ) ?? [],

                ByTheater = theaterAgg?.Buckets.ToDictionary(
                    b => b.Key.ToString(),
                    b => b.DocCount
                ) ?? []
            };
        }
        public async Task<IEnumerable<Report>> FullSearch(
            string? text, string? sector, string? theater, 
            string? location, string? priority, string? reportType, 
            DateTime? from, DateTime? to)
        {
            var cond = new List<Action<QueryDescriptor<Report>>>();

            if (!string.IsNullOrWhiteSpace(text))
            {
                cond.Add(q => q.MatchPhrase(t => t.Field(f => f.Message).Query(text)));
            }
            if (!string.IsNullOrWhiteSpace(sector))
            {
                cond.Add(q => q.Term(t => t.Field(f => f.Sector).Value(sector)));
            }
            if (!string.IsNullOrWhiteSpace(theater))
            {
                cond.Add(q => q.Term(t => t.Field(f => f.Theater).Value(theater)));
            }
            if (!string.IsNullOrWhiteSpace(location))
            {
                cond.Add(q => q.Term(t => t.Field(f => f.Location).Value(location)));
            }
            if (!string.IsNullOrWhiteSpace(priority))
            {
                cond.Add(q => q.Term(t => t.Field(f => f.Priority).Value(priority)));
            }
            if (!string.IsNullOrWhiteSpace(priority))
            {
                cond.Add(q => q.Term(t => t.Field(f => f.Priority).Value(priority)));
            }
            if (!string.IsNullOrWhiteSpace(reportType))
            {
                cond.Add(q => q.Term(t => t.Field(f => f.ReportType).Value(reportType)));
            }
            if(from.HasValue)
            {
                cond.Add(q => q.Range(r => r.Date(d => d.Field(f => f.Timestamp).Gte(from))));
            }
            if(to.HasValue)
            {
                cond.Add(q => q.Range(r => r.Date(d => d.Field(f => f.Timestamp).Lte(to))));
            }

            var response = await _client.SearchAsync<Report>(s => s
                .Indices(_indexName)
                .Size(1000)
                .Query(q => q.Bool( b=> b.Filter(cond.ToArray()))));

            return response.Documents;
        }
    }    
}