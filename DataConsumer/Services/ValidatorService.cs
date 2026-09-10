using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataConsumer.Models;
using Microsoft.Extensions.Logging;

namespace DataConsumer.Services
{
    public class ValidatorService
    {
        private readonly ILogger<ValidatorService> _logger;
        private readonly List<string> _validPriority = [
            "Low", "Medium", "High", "Critical" 
        ];

        private readonly List<string> _validReportType = [
            "Observation", "Movement", "Meeting", "Access", "Communication", "Logistics", "Incident"
        ];
        public ValidatorService(ILogger<ValidatorService> logger)
        {
            _logger = logger;
        }
        public bool ValidateData(ReportElastic report)
        {
            if(!_validPriority.Contains(report.Priority))
            {
                _logger.LogInformation("Invalid validate: Invalid Priority");
                return false;
            }
            if(!_validReportType.Contains(report.ReportType))
            {
                _logger.LogInformation("Invalid validate: Invalid ReportType");
                return false;
            }
            if(report.SubjectType == null && report.SubjectId != null)
            {
                _logger.LogInformation("SubjectType or SubjectId is NULL");
                return false;
            }
            if(report.SubjectType != null && report.SubjectId == null)
            {
                _logger.LogInformation("SubjectType or SubjectId is NULL");
                return false;
            }
            if (!CheckWhiteSpaceOrNull(report))
            {
                _logger.LogInformation("Required field is NULL");
                return false;
            }
            return true;
        }

        public bool CheckWhiteSpaceOrNull(ReportElastic report)
        {
            if (string.IsNullOrWhiteSpace(report.AgentId))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(report.Location))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(report.Message))
            {
                return false;
            }if (string.IsNullOrWhiteSpace(report.Priority))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(report.ReportId))
            {
                return false;
            }if (string.IsNullOrWhiteSpace(report.ReportType))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(report.Sector))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(report.SourceType))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(report.Theater))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(report.Unit))
            {
                return false;
            }
            return true;
        }
    }
}