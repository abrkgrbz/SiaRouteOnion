using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Commands.ProjectOfficers.DeleteProjectOfficers
{
    public class DeleteProjectOfficersQuery : IRequest<bool>
    {
        public string UserId { get; set; }
        public int ProjectId { get; set; }
    }

    public class DeleteProjectOfficersQueryHandler : IRequestHandler<DeleteProjectOfficersQuery, bool>
    {
        private readonly IProjectOfficersRepositoryAsync _projectOfficersRepositoryAsync;

        public DeleteProjectOfficersQueryHandler(IProjectOfficersRepositoryAsync projectOfficersRepositoryAsync)
        {
            _projectOfficersRepositoryAsync = projectOfficersRepositoryAsync;
        }

        public async Task<bool> Handle(DeleteProjectOfficersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projectOfficer = await _projectOfficersRepositoryAsync.GetAsync(x =>
                    x.UserId == request.UserId && x.ProjectId == request.ProjectId);

                bool response = await _projectOfficersRepositoryAsync.DeleteAsync(projectOfficer, cancellationToken);
                return response;
            }
            catch (Exception e)
            {
                throw new ApiException("Beklenmedik bir hata meydana geldi!");
            }
        }
    }
}
