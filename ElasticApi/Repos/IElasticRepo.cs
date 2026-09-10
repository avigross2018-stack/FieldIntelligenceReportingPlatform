using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticApi.Models;

namespace ElasticApi.Repos
{
    public interface IElasticRepo
    {
        Task<IEnumerable<Report>> SearchMessage(string text);
        Task<IEnumerable<Report>> SearchBySubjectId(string subjectId);
        Task<IEnumerable<Report>> SearchByActivityArea(
            string? sector,
            string? theater,
            string? location
            );
        Task<IEnumerable<Report>> SearchByPriority(
            string? priority, 
            DateTime? from, 
            DateTime? to);
    }
}