using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Queries.Dashboard.GetProjectStatusAnalysis;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Behaviours
{
    public class CacheBehavior<TRequest,TResponse>:IPipelineBehavior<TRequest,TResponse> where TRequest:IRequest<TResponse>
    {
        private readonly IMemoryCache _cache;

        public CacheBehavior(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is GetProjectStatusAnalysisQuery)
            {
                var cacheKey = $"ProjectStatusSummary_{request.GetHashCode()}";
                 
                if (_cache.TryGetValue(cacheKey, out TResponse cachedResponse))
                { 
                    return cachedResponse;
                }
                 
                var response = await next(); 
                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(30));

                return response;
            }
             
            return await next();
        }
    }
}
