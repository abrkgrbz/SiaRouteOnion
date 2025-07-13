
using Application.Features.Commands.ProjectOfficers.CreateProjectOfficers;
using Application.Features.Commands.ProjectOfficers.DeleteProjectOfficers;
using Application.Features.Queries.ProjectOfficer.GetAvailableUsers;
using Application.Features.Queries.ProjectOfficer.GetProjectUsers;
using Microsoft.AspNetCore.Mvc;

namespace SiaRoute.WebApp.Controllers
{
    public class ProjectUserController : BaseController
    {
        /// <summary>
        /// Projedeki kullanıcıları listeler
        /// </summary>
        [HttpGet("get-available-users")]
        public async Task<IActionResult> GetAvailableUsers(int projectId, int pageNumber = 1, int pageSize = 3, string search = "")
        {
            var result = await Mediator.Send(new GetAvailableUsersQuery(){ProjectId = projectId,PageNumber = pageNumber,PageSize = pageSize,SearchTerm = search});
            return Json(new { success = true, data = result });
        }

        /// <summary>
        /// Projedeki kullanıcıları listeler
        /// </summary>
        [HttpGet("get-user-project-list")]
        public async Task<IActionResult> GetProjectUsers(int projectId, int pageNumber = 1, int pageSize = 10, string search = "")
        {
            var result = await Mediator.Send(new GetProjectUsersQuery() { ProjectId = projectId, PageNumber = pageNumber, PageSize = pageSize, SearchTerm = search });
            return Json(new { success = true, data = result });
        }

        /// <summary>
        /// Kullanıcı projeye ekleme işlemi
        /// </summary>
        [HttpPost("add-user-project")]
        public async Task<IActionResult> AddUserToProject([FromBody]CreateProjectOfficersQuery request)
        {
            var response = await Mediator.Send(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Kullanıcıyı projeden çıkarma işlemi
        /// </summary>
        [HttpPost("delete-user-project")]
        public async Task<IActionResult> RemoveUserFromProject([FromBody] DeleteProjectOfficersQuery request)
        {
            var result = await Mediator.Send(request);

            return Json(new { success = result });
        }
    }
}
