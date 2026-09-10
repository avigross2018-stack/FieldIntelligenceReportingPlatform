using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;

namespace ElasticApi.Exceptions
{
    public class GlobalErrorHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalErrorHandler> _logger;
        public GlobalErrorHandler(ILogger<GlobalErrorHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if(exception is InvalidOperationException op)
            {
                _logger.LogError(exception, "Invalid Operation");
                httpContext.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;

                await httpContext.Response.WriteAsJsonAsync(new { error= "Invalid Operation"}, cancellationToken);
                return true;
            }
            if(exception is InvalidElasticInteractionException es)
            {
                _logger.LogError(exception, "Elastic error");
                httpContext.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;

                await httpContext.Response.WriteAsJsonAsync( new{ error= "Elastic error"}, cancellationToken);
                return true;
            }

            _logger.LogError(exception, "Unexpected error");
            await httpContext.Response.WriteAsJsonAsync( new { error= "Unexpected error"}, cancellationToken);
            return true;
        }
    }
}