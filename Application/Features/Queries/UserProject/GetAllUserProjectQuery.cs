using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Queries.UserProject
{
    public class GetAllUserProjectQuery:IRequest<Response<GetAllProjectResponse>>
    {
        public int ProjectId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
       
    }

    public class GetAllUserProjectQueryHandler : IRequestHandler<GetAllUserProjectQuery, Response<GetAllProjectResponse>>
    {
        private readonly IProjectOfficersRepositoryAsync _projectOfficersRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IUserProjectRepositoryAsync _userProjectRepository;
        private readonly IUserService _userService;
        public GetAllUserProjectQueryHandler(  IMapper mapper, IUserProjectRepositoryAsync userProjectRepository, IUserService userService, IProjectOfficersRepositoryAsync projectOfficersRepositoryAsync)
        {
            _mapper = mapper;
            _userProjectRepository = userProjectRepository;
            _userService = userService;
            _projectOfficersRepositoryAsync = projectOfficersRepositoryAsync;
        }

        public async Task<Response<GetAllProjectResponse>> Handle(GetAllUserProjectQuery request, CancellationToken cancellationToken)
        { 
            List<GetAllUserProjectQueryViewModel> model = new List<GetAllUserProjectQueryViewModel>();
            var userProjects = await _userProjectRepository.GetWhereList(x => x.ProjectId == request.ProjectId); 
            var allUsers = await _userService.GetAllUser();
            var projectOfficers =
                await _projectOfficersRepositoryAsync.GetWhereList(x => x.ProjectId == request.ProjectId);
            if (projectOfficers.Count>0)
            {
                foreach (var projectOfficer in projectOfficers)
                {
                    var userResponse = allUsers.FirstOrDefault(x => x.Id == projectOfficer.UserId);
                    if (userResponse is not null)
                    {
                        model.Add(new GetAllUserProjectQueryViewModel()
                        {
                            Email = userResponse.Email,
                            Fullname = userResponse.FullName,
                            Id = userResponse.Id

                        });
                    }

                }
            }
       
            foreach (var userProject in userProjects)
            {
                var userResponse = allUsers.FirstOrDefault(x => x.Id == userProject.UserId);
                if (userResponse is not null)
                {
                    model.Add(new GetAllUserProjectQueryViewModel()
                    {
                        Email = userResponse.Email,
                        Fullname = userResponse.FullName,
                        Id = userResponse.Id

                    });
                }
               
            }
            if (model.Any())
            {
                int totalCount = model.Count;
                model = model.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                var response = new GetAllProjectResponse() { Users = model, TotalCount = totalCount };
                return new Response<GetAllProjectResponse>(response);
            }
            return new Response<GetAllProjectResponse>("Kullanıcı bulunamadı");

        }
       

        
    }
}
