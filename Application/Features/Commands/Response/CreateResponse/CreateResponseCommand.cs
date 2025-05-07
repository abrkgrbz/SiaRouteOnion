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

namespace Application.Features.Commands.Response.CreateResponse
{
    public partial class CreateResponseCommand:IRequest<Response<bool>>
    {
        public List<Domain.Entities.Response> responses { get; set; }
        public int ProjectId { get; set; }
    }

    public class CreateResponseCommandHandler : IRequestHandler<CreateResponseCommand, Response<bool>>
    {
        private readonly IResponseRepositoryAsync _responseRepository;
        private readonly IMapper _mapper;

        public CreateResponseCommandHandler(IResponseRepositoryAsync responseRepository, IMapper mapper)
        {
            _responseRepository = responseRepository;
            _mapper = mapper;
        }

        public async Task<Response<bool>> Handle(CreateResponseCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.responses)
            {
                item.ProjectId = request.ProjectId;
            }
            var response = await _responseRepository.AddRangeAsync(request.responses, cancellationToken);
            if (!response)
            {
                throw new ApiException("Response tablo oluşturuken beklenmedik bir hata oluştu!");
            }
            return new Response<bool>(response);
        }
    }
}
