using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Commands.ProjectOfficers.CreateProjectOfficers
{
    public class CreateProjectOfficersQuery : IRequest<CreateProjectOfficersResponse>
    {
        public string UserId { get; set; }
        public int ProjectId { get; set; } 
    }

    public class CreateProjectOfficersResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class
        CreateProjectOfficersQueryHandler : IRequestHandler<CreateProjectOfficersQuery, CreateProjectOfficersResponse>
    {
        private readonly IProjectOfficersRepositoryAsync _projectOfficersRepositoryAsync;

        public CreateProjectOfficersQueryHandler(IProjectOfficersRepositoryAsync projectOfficersRepositoryAsync)
        {
            _projectOfficersRepositoryAsync = projectOfficersRepositoryAsync;
        }

        public async Task<CreateProjectOfficersResponse> Handle(CreateProjectOfficersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _projectOfficersRepositoryAsync.AddAsync(
                    new Domain.Entities.ProjectOfficers()
                    {
                        ProjectId = request.ProjectId,
                        UserId = request.UserId,
                        Created = DateTime.Now
                    }, cancellationToken);

                if (data is not null)
                {
                    return new CreateProjectOfficersResponse()
                    {
                        Success = true,
                        Message = "Kullanıcı başarıyla projeye eklendi"
                    };
                }

                return new CreateProjectOfficersResponse()
                {
                    Success = false,
                    Message = "Kullanıcı eklenirken bir hata oluştu"
                };
            }
            catch (Exception e)
            {
                throw new ApiException("Beklenmedik bir hata meydana geldi!");
            }
        }
    }
}
