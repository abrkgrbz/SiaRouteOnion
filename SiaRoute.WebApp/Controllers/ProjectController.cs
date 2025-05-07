
#region Using Tags

using Application.Features.Commands.PrintStudy.CreatePrintStudy;
using Application.Features.Commands.Project.CreateProject;
using Application.Features.Commands.Question.CreateQuestion;
using Application.Features.Commands.Response.CreateResponse;
using Application.Features.Commands.UserProject.CreateUserProject;
using Application.Features.Commands.UserProject.DeleteUserProject;
using Application.Features.Queries.PrintStudy;
using Application.Features.Queries.Project.GetAllProject;
using Application.Features.Queries.Project.GetProjectDetails;
using Application.Features.Queries.User.GetAllUser;
using Application.Features.Queries.User.GetUserByName;
using Application.Features.Queries.UserProject;
using Application.Interfaces;
using Application.Interfaces.SSIServices;
using ClosedXML.Excel;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiaRoute.WebApp.Helpers;
using System.IO;
using Application.Features.Commands.Project.DeleteProject;
using Application.Features.Commands.Project.UpdateProject;
using Application.Features.Commands.ProjectMethod.CreateProjectMethod;
using Application.Features.Commands.ProjectMethod.UpdateProjectMethod;
using Application.Features.Commands.ProjectNote;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Application.Features.Commands.ProjectProcess.UpdateCompletedProcess;
using Application.Features.Commands.ProjectProcess.UpdatePlannedProcess;
using Application.Features.Queries.ProjectMethod.GetProjectMethod;
using Application.Features.Queries.ProjectNote;
using Application.Features.Queries.ProjectOfficer.GetProjectOfficerByProjectId;
using Application.Features.Queries.ProjectProcess.GetProjectProcessByProjectId;
using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using SiaRoute.WebApp.Models;

#endregion

namespace SiaRoute.WebApp.Controllers
{
    [Authorize(Policy = "BasicUserPolicy")]
    public class ProjectController : ProjectBaseController
    {
        private ISSIDataService _ssiDataService;
        private readonly IWebHostEnvironment _environment;
        private readonly IExcelService _excelService; 
        public ProjectController(IWebHostEnvironment environment, IExcelService excelService, IPrintStudyService printStudyService, ISSIDataService ssiDataService) : base(printStudyService)
        {
            _environment = environment;
            _excelService = excelService; 
            _ssiDataService = ssiDataService;
        }

        [HttpGet("project-list")]
        public IActionResult ProjectList()
        {
            return View();
        }

        [HttpPost("project-list/proje-olustur")]
        public async Task<IActionResult> CreateProject(CreateProjectCommand command)
        {
            if (Request.Form.ContainsKey("ProjectSizes"))
            {
                var projectSizesJson = Request.Form["ProjectSizes"];
                command.ProjectSizes = JsonConvert.DeserializeObject<Dictionary<string, string>>(projectSizesJson);
            }
            var result = await Mediator.Send(command);
            return Ok(result.Succeeded); 
        }

        [HttpPost("project-list/LoadTable")]
        public async Task<IActionResult> GetProjectList(GetAllProjectQuery getAllProject)
        {
            getAllProject.orderColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
            getAllProject.orderDir = Request.Form["order[0][dir]"].FirstOrDefault();
            getAllProject.orderColumnName =
                Request.Form[$"columns[{getAllProject.orderColumnIndex}][name]"].FirstOrDefault();
            getAllProject.searchValue = Request.Form["search[value]"].FirstOrDefault();
            var response = await Mediator.Send(getAllProject);
            return Ok(response);
        }


        [HttpGet("project-details")]
        public async Task<IActionResult> ProjectDetails(int id)
        {
            ViewBag.ActivePage = "ProjectDetails"; 
            await ProjectPartial(id);
            return View();
        }

   

        [HttpGet("project-targets")]
        public async Task<IActionResult> ProjectTargets(int id)
        {
            ViewBag.ActivePage = "ProjectTargets";
            await ProjectPartial(id);
            return View();
        }

        [HttpGet("project-settings")]
        public async Task<IActionResult> ProjectSettings(int id)
        {
            ViewBag.ActivePage = "ProjectSettings";
            await ProjectPartial(id);
            var data = await Mediator.Send(new GetProjectDetailsQuery() { Id = id });
            return View(data.Data);
        }

        [HttpGet("project-users")]
        public async Task<IActionResult> ProjectUsers(int id)
        {
            ViewBag.ActivePage = "ProjectUsers";
            await ProjectPartial(id);
            return View();
        }


        [HttpPost("project-users/LoadTable")]
        public async Task<IActionResult> GetUsersList(GetAllUserQuery getAllUser)
        {
            getAllUser.orderColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
            getAllUser.orderDir = Request.Form["order[0][dir]"].FirstOrDefault();
            getAllUser.orderColumnName =
                Request.Form[$"columns[{getAllUser.orderColumnIndex}][name]"].FirstOrDefault();
            getAllUser.searchValue = Request.Form["search[value]"].FirstOrDefault();
            var response = await Mediator.Send(getAllUser);
            return Ok(response);
        }

        [HttpPost("add-user-project")]
        public async Task<IActionResult> AddUserProject([FromBody] AssignUsersRequest model)
        {
            var response = await Mediator.Send(new CreateUserProjectCommand() { ProjectId = model.ProjectId, UserId = model.UserIds });
            return Ok(response);
        }

        public class AssignUsersRequest
        {
            public int ProjectId { get; set; }
            public List<string> UserIds { get; set; }
        }

        [HttpGet("get-user-project-list")]
        public async Task<IActionResult> GetUserListByProject([FromQuery] GetAllUserProjectQuery filter)
        {
            var response = await Mediator.Send(filter);
            return Json(response);
        }
        [HttpGet("project-file-list")]
        public async Task<IActionResult> ProjectFileList(int id)
        {
            var data = await Mediator.Send(new GetPrintStudyDetailsByProjectIdQuery() { ProjectId = id });
            ViewBag.ActivePage = "ProjectFileList";
            if (data.Data is not null)
            {
                ViewBag.ProjectName = data.Data.ProjectCode;
                var createdDate = data.Data.Created;
                var currentDate = DateTime.Now;
                var difference = (currentDate - createdDate).Days;
                ViewBag.CreatedDateDifference = difference;
            }
            await ProjectPartial(id);
            return View();
        }

        [HttpPost("delete-user-project")]
        public async Task<IActionResult> DeleteUserProject([FromBody] DeleteUserProjectCommand model)
        {
            var data = await Mediator.Send(model);
            return Ok(data);
        }

        [HttpPost("upload-project-file")]
        public async Task<IActionResult> UploadProjectFile(IFormFile printStudyFile, [FromForm] int projectId)
        {

            string targetFolder = Path.Combine(_environment.WebRootPath, "Uploads/PrintStudyFile");
            var uploadFile = await _excelService.UploadFile(printStudyFile, targetFolder);
            var addPrintStudy = await Mediator.Send(new CreatePrintStudyCommand()
            {
                ProjectId = projectId,
                FileName = uploadFile.fileName,
                FilePath = uploadFile.filePath,
                ProjectCode = uploadFile.pCode,
                FileExtension = ".txt"
            });
            if (addPrintStudy.Succeeded)
            {
                var lists = _printStudyService.GetQuestionResponseList(uploadFile.filePath);
                var question = await Mediator.Send(new CreateQuestionCommand()
                { questions = lists.questions, ProjectId = projectId });
                var response = await Mediator.Send(new CreateResponseCommand()
                { responses = lists.responses, ProjectId = projectId });
            }

            return Ok();

        }


        [HttpGet("download-project-file")]
        public async Task<IActionResult> DownloadProjectFile(int projectId)
        {
            var projectCode = await Mediator.Send(new GetProjectDetailsQuery() { Id = projectId });
            string fileDownloadName = $"{projectCode.Data.ProjectName}.xlsx";
            var dataList = await _ssiDataService.ConvertDataToDataTable(projectCode.Data.Id, projectCode.Data.ProjectName);
            var dataTable = _ssiDataService.ConvertListToDataTable(dataList);
            using (var wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(dataTable, projectCode.Data.ProjectName);
                var headerRow = worksheet.Row(1);
                foreach (var cell in headerRow.Cells())
                {
                    cell.Value = cell.GetString().Replace(" ", string.Empty);
                }

                headerRow.Height = 20;
                worksheet.Rows().AdjustToContents();
                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
            }
            return Ok();

        }

        [HttpPost("update-project")]
        public async Task<IActionResult> UpdateProject(UpdateProjectCommand updateProjectCommand)
        {
            var response = await Mediator.Send(updateProjectCommand);
            return RedirectToAction("ProjectSettings", "Project", new { id = updateProjectCommand.ProjectId });
        }
        [HttpPost("update-project-planned")]
        public async Task<IActionResult> UpdateProjectPlanned(UpdatePlannedProcessCommand plannedProcessCommand)
        {
            var data = await Mediator.Send(plannedProcessCommand);
            return Ok(data);
        }
        [HttpPost("update-project-completed")]
        public async Task<IActionResult> UpdateProjectCompleted(UpdateProjectCompletedProcessCommand updateProjectCompletedProcessCommand)
        {
            var data = await Mediator.Send(updateProjectCompletedProcessCommand);
            return Ok(data);
        }

        [HttpGet("GetProjectOfficers")]
        public async Task<IActionResult> GetProjectOfficersByProject(int projectId)
        {
            var users = await Mediator.Send(new GetProjectOfficerByProjectIdQuery() { ProjectId = projectId });
            return Ok(users.Data);
        }

        #region Project Note
        //[HttpGet("project-notes")]
        //public async Task<IActionResult> ProjectNotes(int id)
        //{
        //    ViewBag.ActivePage = "ProjectNotes";
        //    await ProjectPartial(id);
        //    return View();
        //}

        //[HttpPost("add-project-notes")]
        //public async Task<IActionResult> AddProjectNote(CreateProjectNoteCommand createProjectNote)
        //{
        //    var response = await Mediator.Send(createProjectNote);
        //    return Ok(response);
        //}

        //[HttpGet("get-all-project-notes")]
        //public async Task<IActionResult> GetAllProjectNoteByProject(GetAllProjectNoteByProjectQuery getAllProjectNoteByProjectQuery)
        //{
        //    var data = await Mediator.Send(getAllProjectNoteByProjectQuery);
        //    return Ok(data);
        //}

        #endregion

        [HttpGet("get-project-details-chart-data")]
        public async Task<IActionResult> GetProjectDetailsChartData()
        {
            string projectName = _printStudyService.ProjectName;
            var data = await _ssiDataService.ProjectStatusDetails(projectName);
            return Ok(data);
        }

        [HttpGet("get-project-methods")]
        public async Task<IActionResult> GetProjectsMethods(int projectId)
        {
            var data = await Mediator.Send(new GetProjectMethodQuery() { ProjectId =  projectId});
            return Ok(data);
        }

        [HttpPost("update-project-methods")]
        public async Task<IActionResult> UpdateProjectMethods(UpdateProjectMethodCommand updateProjectMethodCommand)
        {
            var data = await Mediator.Send(updateProjectMethodCommand);
            return Ok(data);
        }

        [HttpPost("delete-project")]
        public async Task<IActionResult> DeleteProject(int projectId)
        {

            var data = await Mediator.Send(new DeleteProjectCommand() { ProjectId = projectId });
            return Ok();
        }

        [HttpGet("get-project-process")]
        public async Task<IActionResult> GetProjectProcess(int projectId)
        {
            var data = await Mediator.Send(new GetProjectProcessByProjectIdQuery() { ProjectId = projectId });
            return Ok(data);
        }

        [HttpPost("add-project-method")]
        public async Task<IActionResult>? AddProjectMethod(AddProjectMethod model)
        {
            var data = await Mediator.Send(new CreateProjectMethodCommand()
            {
                MethodName = model.MethodName,
                ProjectId = model.ProjectId,
                Size = model.Size
            });
            return Ok(data);
        }

        #region Revize

        //[HttpGet]
        //public async Task<IActionResult> ProjectPartial(int id)
        //{
        //    var data = await Mediator.Send(new GetProjectDetailsQuery() { Id = id });
        //    _printStudyService.SetProjectName(data.Data.ProjectName);
        //    return PartialView("_ProjectDetailPartialView", data.Data);
        //}


        #endregion
    }
}
