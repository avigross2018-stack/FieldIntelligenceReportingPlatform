using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using ElasticApi.Models;
using ElasticApi.Repos;
using Microsoft.AspNetCore.Mvc;

namespace ElasticApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IElasticRepo _elasticRepo;
        private readonly ILogger<ReportsController> _logger;
        public ReportsController(IElasticRepo elasticRepo, ILogger<ReportsController> logger)
        {
            _elasticRepo = elasticRepo;
            _logger = logger;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Report>>> SearchMessage(string text)
        {
            return Ok(await _elasticRepo.SearchMessage(text));
        }

        [HttpGet("subjects/{subjectId}")]
        public async Task<ActionResult<IEnumerable<Report>>> SearchBySubjectId(string subjectId)
        {
            return Ok(await _elasticRepo.SearchBySubjectId(subjectId));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Report>>> SearchByActionArea(
            string? sector,
            string? theater,
            string? location
            )
        {
            return Ok(await _elasticRepo.SearchByActivityArea(sector, theater, location));
        }

        [HttpGet("by-priority")]
        public async Task<ActionResult<IEnumerable<Report>>> SearchByPriority(
            string? priority,
            DateTime? from,
            DateTime? to
        )
        {
            return Ok(await _elasticRepo.SearchByPriority(priority, from, to));
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetStatistics()
        {
            return Ok(await _elasticRepo.GetStatistics());
        }

        [HttpGet("full-search")]
        public async Task<ActionResult<IEnumerable<Report>>> FullSearch(
            string? text, string? sector, string? theater, 
            string? location, string? priority, string? reportType, 
            DateTime? from, DateTime? to
        )
        {
            return Ok(await _elasticRepo.FullSearch(text, sector, theater, location, priority, reportType, from, to));
        }
    }
}