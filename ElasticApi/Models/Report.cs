using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticApi.Models
{
    public class Report
    {
        [Required(AllowEmptyStrings =false)]
        public string reportId { get; set; }

        [Required(AllowEmptyStrings =false)]
        public DateTime timestamp { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string agentId { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string unit { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string theater { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string sector { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string location { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string reportType { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string priority { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string sourceType { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string message { get; set; }
        
        public string? subjectId { get; set; }
        public string? subjectType { get; set; }        
    }
}