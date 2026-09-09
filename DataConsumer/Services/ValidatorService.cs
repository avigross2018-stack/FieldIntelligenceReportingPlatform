using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataConsumer.Models;

namespace DataConsumer.Services
{
    public class ValidatorService
    {
        private readonly List<string> _validPriority = [
            "Low", "Medium", "High", "Critical" 
        ];

        private readonly List<string> _validReportType = [
            "Observation", "Movement", "Meeting", "Access", "Communication", "Logistics", "Incident"
        ];
        public bool ValidateData(Report report)
        {
            if(!_validPriority.Contains(report.priority))
            {
                return false;
            }
            if(!_validReportType.Contains(report.reportType))
            {
                return false;
            }
            if(report.subjectId == null && report.subjectType != null)
            {
                return false;
            }
            if(report.subjectId != null && report.subjectType == null)
            {
                return false;
            }
            return true;
        }
    }
}