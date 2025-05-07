using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.SSIServices;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repositories;
using Persistence.Repositories.SSI;

namespace Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SiaRouteDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("SiaRouteConnection"),
                    b => b.MigrationsAssembly(typeof(SiaRouteDbContext).Assembly.FullName)));
            services.AddDbContext<SiaRouteSSIDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("SiaRouteSSIDbConnection"),
                    b => b.MigrationsAssembly(typeof(SiaRouteSSIDbContext).Assembly.FullName)));
            services.AddMemoryCache();
            #region Repositories
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddTransient<IQuestionRepositoryAsync, QuestionRepositoryAsync>();
            services.AddTransient<IResponseRepositoryAsync, ResponseRepositoryAsync>();
            services.AddTransient<IPrintStudyRepositoryAsync, PrintStudyRepositoryAsync>(); 
            services.AddTransient<IProjectRepositoryAsync, ProjectRepositoryAsync>();
            services.AddTransient<IUserProjectRepositoryAsync, UserProjectRepositoryAsync>();
            services.AddScoped<ISSIDataService, SSIDataService>();
            services.AddTransient<ISCMRepositoryAsync, SCMRepositoryAsync>();
            services.AddTransient<IProjectSCMRepositoryAsync, ProjectSCMRepositoryAsync>();
            services.AddTransient<IProjectNoteRepositoryAsync, ProjectNoteRepositoryAsync>();
            services.AddTransient<IProjectProcessRepositoryAsync, ProjectProcessRepositoryAsync>();
            services.AddTransient<IProjectOfficersRepositoryAsync, ProjectOfficersRepositoryAsync>();
            services.AddTransient<IProjectMethodsRepositoryAsync, ProjectMethodsRepositoryAsync>();
            #endregion
        }
    }
}
