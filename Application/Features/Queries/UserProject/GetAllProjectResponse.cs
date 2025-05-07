using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Queries.UserProject
{
    public class GetAllProjectResponse
    {
        public IReadOnlyList<GetAllUserProjectQueryViewModel> Users { get; set; }
        public int TotalCount { get; set; }
    }
}
