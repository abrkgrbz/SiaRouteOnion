using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Entities;
using MediatR;

namespace Application.Features.Commands.ProjectMethod.UpdateProjectMethod
{
     
    public class UpdateProjectMethodCommand:IRequest<Response<bool>>
    {
        public int projectId { get; set; }
        public List<int> Ids { get; set; }
        public List<int> Sizes { get; set; }
    }

    public class UpdateProjectMethodCommandHandler : IRequestHandler<UpdateProjectMethodCommand, Response<bool>>
    {
        private readonly IProjectMethodsRepositoryAsync _projectMethodsRepositoryAsync;

        public UpdateProjectMethodCommandHandler(IProjectMethodsRepositoryAsync projectMethodsRepositoryAsync)
        {
            _projectMethodsRepositoryAsync = projectMethodsRepositoryAsync;
        }

        public async Task<Response<bool>> Handle(UpdateProjectMethodCommand request, CancellationToken cancellationToken)
        { 
            for (int i = 0; i < request.Ids.Count; i++)
            {
                var id = request.Ids[i];
                var method = await _projectMethodsRepositoryAsync.GetByIdAsync(id);
                method.Size = request.Sizes[i]; 
                 await _projectMethodsRepositoryAsync.UpdateAsync(method,cancellationToken);
                 
            }

            return new Response<bool>(true);
        }
    }
}
