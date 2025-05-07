using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services;

namespace Shared
{
    public static class ServiceRegistration
    {
        public static void AddSharedInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IDateTimeService, DateTimeService>(); 
            services.AddSingleton<IPrintStudyService, PrintStudyService>();
            services.AddTransient<IRegularExpressionService, RegularExpressionService>();
            services.AddTransient<IExcelService, ExcelService>();
        }
    }
}
