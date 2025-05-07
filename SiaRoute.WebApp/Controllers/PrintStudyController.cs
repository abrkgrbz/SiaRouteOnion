using Application.Features.Commands.PrintStudy.CreatePrintStudy;
using Application.Features.Commands.Question.CreateQuestion;
using Application.Features.Commands.Response.CreateResponse;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SiaRoute.WebApp.Controllers
{
    [Authorize]
    public class PrintStudyController : BaseController
    {
        private IWebHostEnvironment _environment;
        private IPrintStudyService _printStudyService;
        private IExcelService _excelService;
        public PrintStudyController(IPrintStudyService printStudyService, IExcelService excelService, IWebHostEnvironment environment)
        {
            _printStudyService = printStudyService;
            _excelService = excelService;
            _environment = environment;
        }

        public IActionResult UploadPrintStudy()
        {
            return View();
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> StreamRead(IFormFile file)
        {
            var targetfolder = Path.Combine(_environment.WebRootPath, "Uploads");
            var data = await _excelService.UploadFile(file, targetfolder);
            var lists = _printStudyService.GetQuestionResponseList(data.filePath);
            var printStudy = await Mediator.Send(new CreatePrintStudyCommand()
            { FileName = data.fileName, FilePath = data.filePath, ProjectCode = data.pCode });
            if (printStudy.Succeeded)
            {
                var question = await Mediator.Send(new CreateQuestionCommand() { questions = lists.questions });
                var response = await Mediator.Send(new CreateResponseCommand() { responses = lists.responses });
            }

            return Ok();

        }
    }
}
