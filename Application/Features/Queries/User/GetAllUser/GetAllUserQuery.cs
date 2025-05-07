using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Queries.User.GetAllUser
{

    public class GetAllUserQuery:IRequest<DataTableReponse<IReadOnlyList<GetAllUserQueryViewModel>>>
    {
        public int draw { get; set; }
        public int Length { get; set; }
        public int Start { get; set; }
        public string? orderColumnIndex { get; set; }
        public string? orderDir { get; set; }
        public string? orderColumnName { get; set; }
        public string? searchValue { get; set; }
        public int projectId { get; set; }
    }

    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, DataTableReponse<IReadOnlyList<GetAllUserQueryViewModel>>>
    {
      
        private readonly IUserService _userService;
        private readonly IProjectOfficersRepositoryAsync _projectOfficersRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IUserProjectRepositoryAsync _userProjectRepository;
        public GetAllUserQueryHandler(IUserService userService, IMapper mapper, IUserProjectRepositoryAsync userProjectRepository, IProjectOfficersRepositoryAsync projectOfficersRepositoryAsync)
        {
            _userService = userService;
            _mapper = mapper;
            _userProjectRepository = userProjectRepository;
            _projectOfficersRepositoryAsync = projectOfficersRepositoryAsync;
        }
        public async Task<DataTableReponse<IReadOnlyList<GetAllUserQueryViewModel>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        { 
            var users = await _userService.GetQueryableAllUser(); 
            
            int recordsFiltered = 0, recordTotal = 0;
            if (!string.IsNullOrEmpty(request.searchValue))
            {
                users =  users.Where(x => x.FullName.Contains(request.searchValue.ToLower()));
            }
            if (!string.IsNullOrEmpty(request.orderColumnName) && !string.IsNullOrEmpty(request.orderDir))
            {
                users = await _userService.OrderByField(users, request.orderColumnName, request.orderDir == "asc");
                recordTotal = users.Count();
                recordsFiltered = users.Count();
            }
            var responseData = users.Skip(request.Start).Take(request.Length).ToList(); 
            var userProjects = await _userProjectRepository.GetWhereList(x => x.ProjectId == request.projectId);
            var userProjectOfficers =
                await _projectOfficersRepositoryAsync.GetWhereList(x => x.ProjectId == request.projectId);
            var filteredUsers = responseData.Where(user => userProjects.All(up => up.UserId != user.Id)).ToList();
            filteredUsers= responseData.Where(user => userProjectOfficers.All(up => up.UserId != user.Id)).ToList();
            var usersViewModel = _mapper.Map<IReadOnlyList<GetAllUserQueryViewModel>>(filteredUsers);
            return new DataTableReponse<IReadOnlyList<GetAllUserQueryViewModel>>(usersViewModel,recordsFiltered, recordTotal);
           
        }
    }
}
