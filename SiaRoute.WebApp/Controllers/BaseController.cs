using Application.Features.Queries.Project.GetProjectDetails;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace SiaRoute.WebApp.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        private ILogger _logger;
        private IMediator _mediator; 
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        protected ILogger Logger => _logger ??= HttpContext.RequestServices.GetService<ILogger>();
        
    }
}
