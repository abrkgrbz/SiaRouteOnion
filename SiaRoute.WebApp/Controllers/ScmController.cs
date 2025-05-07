using Application.Features.Queries.Scm.GetAllScm;
using Microsoft.AspNetCore.Mvc;

namespace SiaRoute.WebApp.Controllers
{
    public class ScmController : BaseController
    {
        [HttpGet("GetAllListSCM")]
        public async Task<IActionResult> GetAllList()
        {
            var data = await Mediator.Send(new GetAllScmQuery());
            return Ok(data.Data);
        }
    }
}
