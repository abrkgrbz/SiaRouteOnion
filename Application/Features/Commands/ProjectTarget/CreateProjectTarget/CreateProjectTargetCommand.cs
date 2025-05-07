using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Commands.ProjectTarget.CreateProjectTarget
{
    public class CreateProjectTargetCommand:IRequest<Response<bool>>
    {

    }
}
