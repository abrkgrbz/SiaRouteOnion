using Application.Features.Commands.ProjectNote;
using Application.Features.Queries.ProjectNote;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SiaRoute.WebApp.Controllers
{
    public class ProjectNoteController : ProjectBaseController
    {
        public ProjectNoteController(IPrintStudyService printStudyService) : base(printStudyService)
        {
        }

        [HttpGet("project-notes")]
        public async Task<IActionResult> ProjectNotes(int id)
        {
            ViewBag.ActivePage = "ProjectNotes";
            await ProjectPartial(id);
            return View();
        }

        [HttpPost("add-project-notes")]
        public async Task<IActionResult> AddProjectNote(CreateProjectNoteCommand createProjectNote)
        {
            var response = await Mediator.Send(createProjectNote);
            return Ok(response);
        }

        [HttpGet("get-all-project-notes")]
        public async Task<IActionResult> GetAllProjectNoteByProject(GetAllProjectNoteByProjectQuery getAllProjectNoteByProjectQuery)
        {
            var data = await Mediator.Send(getAllProjectNoteByProjectQuery);
            return Ok(data);
        }

        [HttpGet("remove-project-note")]
        public async Task<IActionResult> DeleteProjectNote(int id)
        {
            var result =await Mediator.Send(new DeleteProjectNoteCommand() { Id = id });
            return Ok(result);
        }

        [HttpPost("update-project-note")]
        public async Task<IActionResult> UpdateProjectNote(UpdateProjectNoteCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
