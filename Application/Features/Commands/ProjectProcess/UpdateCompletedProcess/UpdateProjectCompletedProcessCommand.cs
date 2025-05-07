using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Commands.ProjectProcess.UpdateCompletedProcess
{
    public class UpdateProjectCompletedProcessCommand : IRequest<Response<bool>>
    {
        public DateTime? GerceklesenScriptTeslim { get; set; }
        public DateTime? GerceklesenScriptKontrol { get; set; }
        public DateTime? GerceklesenScriptRevizyon { get; set; }
        public DateTime? GerceklesenSahaBaslangic { get; set; }
        public DateTime? GerceklesenSahaBitis { get; set; }
        public DateTime? GerceklesenKodlamaTeslim { get; set; }
        public DateTime? GerceklesenTablolamaTeslim { get; set; }
        public DateTime? GerceklesenSoruFormuTeslim { get; set; }
        public DateTime? GerceklesenRaporTeslim { get; set; }
        public int ProjectId { get; set; }
    }

    public class UpdateProjectCompledtedProcessCommandHandler : IRequestHandler<UpdateProjectCompletedProcessCommand, Response<bool>>
    {
        private readonly IProjectProcessRepositoryAsync _projectProcessRepositoryAsync;
        public UpdateProjectCompledtedProcessCommandHandler(IProjectProcessRepositoryAsync projectProcessRepositoryAsync, IMapper mapper)
        {
            _projectProcessRepositoryAsync = projectProcessRepositoryAsync;

        }

        public async Task<Response<bool>> Handle(UpdateProjectCompletedProcessCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _projectProcessRepositoryAsync.GetAsync(x => x.ProjectId == request.ProjectId);
                data.GerceklesenKodlamaTeslim = ConvertToUtc(request.GerceklesenKodlamaTeslim);
                data.GerceklesenRaporTeslim = ConvertToUtc(request.GerceklesenRaporTeslim);
                data.GerceklesenSahaBaslangic = ConvertToUtc(request.GerceklesenSahaBaslangic);
                data.GerceklesenSahaBitis = ConvertToUtc(request.GerceklesenSahaBitis);
                data.GerceklesenScriptKontrol = ConvertToUtc(request.GerceklesenScriptKontrol);
                data.GerceklesenScriptRevizyon = ConvertToUtc(request.GerceklesenScriptRevizyon);
                data.GerceklesenScriptTeslim = ConvertToUtc(request.GerceklesenScriptTeslim);
                data.GerceklesenSoruFormuTeslim = ConvertToUtc(request.GerceklesenSoruFormuTeslim);
                data.GerceklesenTablolamaTeslim = ConvertToUtc(request.GerceklesenTablolamaTeslim);

                data.PlanlananKodlamaTeslim = data.PlanlananKodlamaTeslim;
                data.PlanlananRaporTeslim = data.PlanlananRaporTeslim;
                data.PlanlananSahaBaslangic = data.PlanlananSahaBaslangic;
                data.PlanlananSahaBitis = data.PlanlananSahaBitis;
                data.PlanlananScriptKontrol = data.PlanlananScriptKontrol;
                data.PlanlananScriptRevizyon = data.PlanlananScriptRevizyon;
                data.PlanlananScriptTeslim = data.PlanlananScriptTeslim;
                data.PlanlananSoruFormuTeslim = data.PlanlananSoruFormuTeslim;
                data.PlanlananTablolamaTeslim = data.PlanlananTablolamaTeslim;

                await _projectProcessRepositoryAsync.UpdateAsync(data, cancellationToken);

                return new Response<bool>(true, message: "Güncelleme işlemi başarısız");
            }
            catch (Exception e)
            {
                throw new ApiException(e.Message);
            }
        }

        private DateTime? ConvertToUtc(DateTime? date)
        {
            if (date.HasValue)
            {
                return DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
            }

            return null;
        }
    }

}
