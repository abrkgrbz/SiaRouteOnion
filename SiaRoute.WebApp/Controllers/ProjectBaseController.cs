using Application.Features.Queries.Project.GetProjectDetails;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SiaRoute.WebApp.Controllers
{
    public class ProjectBaseController : BaseController
    {
        protected IPrintStudyService _printStudyService; 
        public ProjectBaseController(IPrintStudyService printStudyService)
        {
            _printStudyService = printStudyService;
        }


        [HttpGet]
        public async Task<IActionResult> ProjectPartial(int id)
        {
            var data = await Mediator.Send(new GetProjectDetailsQuery() { Id = id });
            _printStudyService.SetProjectName(data.Data.ProjectName);
            return PartialView("_ProjectDetailPartialView", data.Data);
        }
    }
}
