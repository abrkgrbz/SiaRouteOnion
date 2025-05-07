using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Commands.PrintStudy.DeletePrintStudy
{
    public class DeletePrintStudyCommand:IRequest<Response<bool>>
    {
        public int ProjectId { get; set; }

    }

    public class DeletePrintStudyCommandHandler : IRequestHandler<DeletePrintStudyCommand, Response<bool>>
    {
        private readonly IPrintStudyRepositoryAsync _printStudyRepositoryAsync;
        private readonly IQuestionRepositoryAsync _questionRepositoryAsync;
        private readonly IResponseRepositoryAsync _responseRepositoryAsync;

        public DeletePrintStudyCommandHandler(IPrintStudyRepositoryAsync printStudyRepositoryAsync, IQuestionRepositoryAsync questionRepositoryAsync, IResponseRepositoryAsync responseRepositoryAsync)
        {
            _printStudyRepositoryAsync = printStudyRepositoryAsync;
            _questionRepositoryAsync = questionRepositoryAsync;
            _responseRepositoryAsync = responseRepositoryAsync;
        }

        public async Task<Response<bool>> Handle(DeletePrintStudyCommand request, CancellationToken cancellationToken)
        {
            var printStudy = await _printStudyRepositoryAsync.GetAsync(x => x.ProjectId == request.ProjectId);
            var questionList = await _questionRepositoryAsync.GetWhereList(x => x.ProjectId == request.ProjectId);
             
            throw new NotImplementedException();
        }
    }
}
