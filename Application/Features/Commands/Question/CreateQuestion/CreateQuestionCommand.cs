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

namespace Application.Features.Commands.Question.CreateQuestion
{

    public partial class CreateQuestionCommand : IRequest<Response<bool>>
    {
        public List<Domain.Entities.Question> questions { get; set; }
        public int ProjectId { get; set; }
    }

    public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Response<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IQuestionRepositoryAsync _questionRepository;

        public CreateQuestionCommandHandler(IQuestionRepositoryAsync questionRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
        }

        public async Task<Response<bool>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.questions)
            {
                item.ProjectId = request.ProjectId;
            }
            var question =await _questionRepository.AddRangeAsync(request.questions,cancellationToken);
            if (!question)
            {
                throw new ApiException("Question tablo oluştururken bir hata oluştu");
            }
            return new Response<bool>(question);
        }
    }
}
