using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Commands.UserProject.CreateUserProject
{
    public class CreateUserProjectCommand:IRequest<Response<bool>>
    {
        public int ProjectId { get; set; }
        public List<string> UserId { get; set; }
        
    }

    public class CreateUserProjectCommandHanlder: IRequestHandler<CreateUserProjectCommand, Response<bool>>
    {
        private readonly IUserProjectRepositoryAsync _userProjectRepository;
        private readonly IMapper _mapper;
        public CreateUserProjectCommandHanlder(IUserProjectRepositoryAsync userProjectRepository, IMapper mapper)
        {
            _userProjectRepository = userProjectRepository;
            _mapper = mapper;
        }
        public async Task<Response<bool>> Handle(CreateUserProjectCommand request, CancellationToken cancellationToken)
        {
            foreach (var userId in request.UserId)
            {
                var userProject = new Domain.Entities.UserProject
                {
                    ProjectId = request.ProjectId,
                    UserId = userId
                };
                await _userProjectRepository.AddAsync(userProject, cancellationToken);
            }
            return new Response<bool>(true);
        }
    }

}
