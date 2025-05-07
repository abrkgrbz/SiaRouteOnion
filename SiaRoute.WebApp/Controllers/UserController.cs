using System.Security.Claims;
using Application.Features.Commands.User.ApproveUser;
using Application.Features.Queries.Project.GetAllProject;
using Application.Features.Queries.User.GetAllUserList;
using Application.Features.Queries.User.GetAllUsersByRoles;
using Microsoft.AspNetCore.Mvc;

namespace SiaRoute.WebApp.Controllers
{
    public class UserController : BaseController
    {

        [HttpGet("user-list")]
        public IActionResult UserList()
        {
            return View();
        }

        [HttpPost("user-list/LoadTable")]
        public async Task<IActionResult> GetUsersAndRolesList(GetAllUsersByRolesQuery getAllUsersByRoles)
        {
            getAllUsersByRoles.orderColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
            getAllUsersByRoles.orderDir = Request.Form["order[0][dir]"].FirstOrDefault();
            getAllUsersByRoles.orderColumnName =
                Request.Form[$"columns[{getAllUsersByRoles.orderColumnIndex}][name]"].FirstOrDefault();
            getAllUsersByRoles.searchValue = Request.Form["search[value]"].FirstOrDefault();
            var response = await Mediator.Send(getAllUsersByRoles);
            return Ok(response);
        }

        [HttpPost("user-approve")]
        public async Task<IActionResult> ApproveUser([FromBody] ApproveUserCommand requestApproveUserCommand)
        {
            var response = await Mediator.Send(requestApproveUserCommand);
            return Ok(response);
        }

        [HttpGet("GetUserListGroupByDepartman")]
        public async Task<IActionResult> GetAllUserGroupByDepartman()
        {
            var data = await Mediator.Send(new GetAllUserListQuery());
            return Ok(data.Data);

        }

        [HttpGet("check-role")]
        public async Task<IActionResult> CheckUserRole()
        { 
            if (User.IsInRole("SuperAdmin"))
            {
                return Ok(new { IsAuthorized = true });
            }
            else
            {
                return Ok(new { IsAuthorized = false });
            }
        }
    }
}
