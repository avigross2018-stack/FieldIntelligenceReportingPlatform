using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ElasticApi.Models
{
    public class Report
    {
        [Required(AllowEmptyStrings =false)]
        public string ReportId { get; set; }

        [Required(AllowEmptyStrings =false)]
        [JsonPropertyName("@timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string AgentId { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string Unit { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string Theater { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string Sector { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string Location { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string ReportType { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string Priority { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string SourceType { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string Message { get; set; }
        
        public string? SubjectId { get; set; }
        public string? SubjectType { get; set; }        
    }
}