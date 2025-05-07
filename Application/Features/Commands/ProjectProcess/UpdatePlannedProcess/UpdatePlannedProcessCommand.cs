using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Commands.ProjectProcess.UpdatePlannedProcess
{
    public class UpdatePlannedProcessCommand:IRequest<Response<bool>>
    {
        public DateTime? PlanlananScriptTeslim { get; set; }
        public DateTime? PlanlananScriptKontrol { get; set; }
        public DateTime? PlanlananScriptRevizyon { get; set; }
        public DateTime? PlanlananSahaBaslangic { get; set; }
        public DateTime? PlanlananSahaBitis { get; set; }
        public DateTime? PlanlananKodlamaTeslim { get; set; }
        public DateTime? PlanlananTablolamaTeslim { get; set; }
        public DateTime? PlanlananSoruFormuTeslim { get; set; }
        public DateTime? PlanlananRaporTeslim { get; set; }
        public int ProjectId { get; set; }
    }

    public class UpdatePlannedProcessCommandHandler : IRequestHandler<UpdatePlannedProcessCommand, Response<bool>>
    {
        private readonly IProjectProcessRepositoryAsync _projectProcessRepositoryAsync;

        public UpdatePlannedProcessCommandHandler(IProjectProcessRepositoryAsync projectProcessRepositoryAsync)
        {
            _projectProcessRepositoryAsync = projectProcessRepositoryAsync;
        }

        public async Task<Response<bool>> Handle(UpdatePlannedProcessCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _projectProcessRepositoryAsync.GetAsync(x => x.ProjectId == request.ProjectId);
                data.PlanlananKodlamaTeslim = ConvertToUtc(request.PlanlananKodlamaTeslim);
                data.PlanlananRaporTeslim =ConvertToUtc(request.PlanlananRaporTeslim);
                data.PlanlananSahaBaslangic =ConvertToUtc( request.PlanlananSahaBaslangic);
                data.PlanlananSahaBitis = ConvertToUtc(request.PlanlananSahaBitis);
                data.PlanlananScriptKontrol = ConvertToUtc(request.PlanlananScriptKontrol);
                data.PlanlananScriptRevizyon = ConvertToUtc(request.PlanlananScriptRevizyon);
                data.PlanlananScriptTeslim = ConvertToUtc(request.PlanlananScriptTeslim);
                data.PlanlananSoruFormuTeslim = ConvertToUtc(request.PlanlananSoruFormuTeslim);
                data.PlanlananTablolamaTeslim = ConvertToUtc(request.PlanlananTablolamaTeslim);


                data.GerceklesenSahaBaslangic = data.GerceklesenSahaBaslangic;
                data.GerceklesenKodlamaTeslim = data.GerceklesenKodlamaTeslim;
                data.GerceklesenRaporTeslim = data.GerceklesenRaporTeslim;
                data.GerceklesenSahaBitis = data.GerceklesenSahaBitis;
                data.GerceklesenScriptKontrol = data.GerceklesenScriptKontrol;
                data.GerceklesenScriptRevizyon = data.GerceklesenScriptRevizyon;
                data.GerceklesenScriptTeslim = data.GerceklesenScriptTeslim;
                data.GerceklesenSoruFormuTeslim = data.GerceklesenSoruFormuTeslim;
                data.GerceklesenTablolamaTeslim = data.GerceklesenTablolamaTeslim;

                await _projectProcessRepositoryAsync.UpdateAsync(data, token: cancellationToken);
                return new Response<bool>(true, message: "Güncelleme işlemi başarılı");
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
