using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using MediatR;

namespace Application.Features.Queries.User.GetAllUsersByRoles
{
    public class GetAllUsersByRolesQuery:IRequest<DataTableReponse<List<UserResponse>>>
    {
        public int draw { get; set; }
        public int Length { get; set; }
        public int Start { get; set; }
        public string? orderColumnIndex { get; set; }
        public string? orderDir { get; set; }
        public string? orderColumnName { get; set; }
        public string? searchValue { get; set; }

    }

    public class GetAllUsersByRolesQueryHandler : IRequestHandler<GetAllUsersByRolesQuery, DataTableReponse<List<UserResponse>>>
    {
        private IUserService _userService;

        public GetAllUsersByRolesQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<DataTableReponse<List<UserResponse>>> Handle(GetAllUsersByRolesQuery request, CancellationToken cancellationToken)
        {
            var users = await _userService.GetQueryableAllUserByRoles();
            int recordsFiltered = 0, recordTotal = 0;
            if (!string.IsNullOrEmpty(request.searchValue))
            {
                users = users.Where(x => x.FullName.Contains(request.searchValue.ToLower()));
            }
            if (!string.IsNullOrEmpty(request.orderColumnName) && !string.IsNullOrEmpty(request.orderDir))
            {
                users = await _userService.OrderByField(users, request.orderColumnName, request.orderDir == "asc");
                recordTotal = users.Count();
                recordsFiltered = users.Count();
            }
            var responseData = users.Skip(request.Start).Take(request.Length).ToList();
            return new DataTableReponse<List<UserResponse>>(responseData, recordsFiltered, recordTotal);
        }
    }
}
