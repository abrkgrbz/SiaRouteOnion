using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Queries.Question.GetAllQuestion
{
    public class GetAllQuestionQuery:IRequest<Response<IEnumerable<Domain.Entities.Question>>>
    {
        public int projectId { get; set; }

    }

    public class GetAllQuestionQueryHandler : IRequestHandler<GetAllQuestionQuery, Response<IEnumerable<Domain.Entities.Question>>>
    {
        private readonly IQuestionRepositoryAsync _questionRepository;
        public GetAllQuestionQueryHandler(IQuestionRepositoryAsync questionRepository)
        {
            _questionRepository = questionRepository;
        }
        public async Task<Response<IEnumerable<Domain.Entities.Question>>> Handle(GetAllQuestionQuery request, CancellationToken cancellationToken)
        {
            var questions = await _questionRepository.GetWhereList(x=>x.ProjectId==request.projectId);
            return new Response<IEnumerable<Domain.Entities.Question>>(questions);
        }
    }
}
