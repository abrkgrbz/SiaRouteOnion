using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class QuestionRepositoryAsync:GenericRepositoryAsync<Question>, IQuestionRepositoryAsync
    {
        private readonly DbSet<Question> _question;
        public QuestionRepositoryAsync(SiaRouteDbContext dbContext) : base(dbContext)
        { 
            _question = dbContext.Set<Question>();
        }
    }
}
