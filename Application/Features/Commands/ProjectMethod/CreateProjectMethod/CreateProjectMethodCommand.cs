using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Commands.ProjectMethod.CreateProjectMethod
{
    public class CreateProjectMethodCommand:IRequest<Response<bool>>
    {
        public int ProjectId { get; set; }
        public string MethodName { get; set; }
        public int Size { get; set; }
    }

    public class CreateProjectMethodCommandHandler:IRequestHandler<CreateProjectMethodCommand,Response<bool>>
    {
        private readonly IProjectMethodsRepositoryAsync _projectMethodsRepositoryAsync;
        private IMapper _mapper;

        public CreateProjectMethodCommandHandler(IProjectMethodsRepositoryAsync projectMethodsRepositoryAsync, IMapper mapper)
        {
            _projectMethodsRepositoryAsync = projectMethodsRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<bool>> Handle(CreateProjectMethodCommand request, CancellationToken cancellationToken)
        {
            var mapping = _mapper.Map<ProjectMethods>(request);
            var data =await _projectMethodsRepositoryAsync.AddAsync(mapping, cancellationToken);
            if (data is not null)
            {
                return new Response<bool>(true);
            }

            return new Response<bool>(false);
        }
    }
}
