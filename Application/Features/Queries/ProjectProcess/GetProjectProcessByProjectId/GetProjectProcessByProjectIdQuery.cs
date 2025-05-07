using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Queries.ProjectProcess.GetProjectProcessByProjectId
{
    public class GetProjectProcessByProjectIdQuery : IRequest<Response<List<GetProjectProcessViewModel>>>
    {
        public int ProjectId { get; set; } 
    }

    public class GetProjectProcessByProjectIdQueryHandler : IRequestHandler<GetProjectProcessByProjectIdQuery,
        Response<List<GetProjectProcessViewModel>>>
    {
        private readonly IProjectProcessRepositoryAsync _projectProcessRepositoryAsync;

        public GetProjectProcessByProjectIdQueryHandler(IProjectProcessRepositoryAsync projectProcessRepositoryAsync)
        {
            _projectProcessRepositoryAsync = projectProcessRepositoryAsync;
        }

        public async Task<Response<List<GetProjectProcessViewModel>>> Handle(GetProjectProcessByProjectIdQuery request, CancellationToken cancellationToken)
        {
            var data = await _projectProcessRepositoryAsync.GetWhereList(x => x.ProjectId == request.ProjectId);
            var transformedData = TransformData(data);
            return new Response<List<GetProjectProcessViewModel>>(transformedData);
        }

        public List<GetProjectProcessViewModel> TransformData(IEnumerable<Domain.Entities.ProjectProcess> data)
        {
            var processList = new List<GetProjectProcessViewModel>();

            foreach (var item in data)
            {
                processList.Add(new GetProjectProcessViewModel
                {
                    ProcessName = "Script Teslim",
                    PlannedDate = item.PlanlananScriptTeslim,
                    RealizedDate = item.GerceklesenScriptTeslim,
                    Status = GetStatus(item.PlanlananScriptTeslim, item.GerceklesenScriptTeslim)
                });
                processList.Add(new GetProjectProcessViewModel
                {
                    ProcessName = "Script Kontrol",
                    PlannedDate = item.PlanlananScriptKontrol,
                    RealizedDate = item.GerceklesenScriptKontrol,
                    Status = GetStatus(item.PlanlananScriptKontrol, item.GerceklesenScriptKontrol)
                });
                processList.Add(new GetProjectProcessViewModel
                {
                    ProcessName = "Script Revizyon",
                    PlannedDate = item.PlanlananScriptRevizyon,
                    RealizedDate = item.GerceklesenScriptRevizyon,
                    Status = GetStatus(item.PlanlananScriptRevizyon, item.GerceklesenScriptRevizyon)
                });
                processList.Add(new GetProjectProcessViewModel
                {
                    ProcessName = "Saha Baslangic",
                    PlannedDate = item.PlanlananSahaBaslangic,
                    RealizedDate = item.GerceklesenSahaBaslangic,
                    Status = GetStatus(item.PlanlananSahaBaslangic, item.GerceklesenSahaBaslangic)
                });
                processList.Add(new GetProjectProcessViewModel
                {
                    ProcessName = "Saha Bitis",
                    PlannedDate = item.PlanlananSahaBitis,
                    RealizedDate = item.GerceklesenSahaBitis,
                    Status = GetStatus(item.PlanlananSahaBitis, item.GerceklesenSahaBitis)
                });
                processList.Add(new GetProjectProcessViewModel
                {
                    ProcessName = "Kodlama Teslim",
                    PlannedDate = item.PlanlananKodlamaTeslim,
                    RealizedDate = item.GerceklesenKodlamaTeslim,
                    Status = GetStatus(item.PlanlananKodlamaTeslim, item.GerceklesenKodlamaTeslim)
                });
                processList.Add(new GetProjectProcessViewModel
                {
                    ProcessName = "Tablolama Teslim",
                    PlannedDate = item.PlanlananTablolamaTeslim,
                    RealizedDate = item.GerceklesenTablolamaTeslim,
                    Status = GetStatus(item.PlanlananTablolamaTeslim, item.GerceklesenTablolamaTeslim)
                });
                processList.Add(new GetProjectProcessViewModel
                {
                    ProcessName = "Soru Formu Teslim",
                    PlannedDate = item.PlanlananSoruFormuTeslim,
                    RealizedDate = item.GerceklesenSoruFormuTeslim,
                    Status = GetStatus(item.PlanlananSoruFormuTeslim, item.GerceklesenSoruFormuTeslim)
                });
                processList.Add(new GetProjectProcessViewModel
                {
                    ProcessName = "Rapor Teslim",
                    PlannedDate = item.PlanlananRaporTeslim,
                    RealizedDate = item.GerceklesenRaporTeslim,
                    Status = GetStatus(item.PlanlananRaporTeslim, item.GerceklesenRaporTeslim)
                });
            }

            return processList;
        }

        public string GetStatus(DateTime? plannedDate, DateTime? realizedDate)
        {
            if (!realizedDate.HasValue)
            {
                return "Gerçekleşmedi";
            } 
            return realizedDate <= plannedDate ? "Zamanında" : "Gecikmeli";
        }
    }
}
