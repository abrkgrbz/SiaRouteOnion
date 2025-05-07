using Application.Enums;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Persistence.Contexts;

namespace Persistence.Seeds
{
    public class DefaultSCM
    {
        public static async Task SeedAsync(SiaRouteDbContext dbContext)
        {
            if (!dbContext.SCM.Any())
            {
                var scms = new List<SCM> {

                    new SCM(){SCMName= SCMs.Sia.ToString(),Active = true},
                    new SCM(){SCMName= SCMs.Diyalog.ToString(),Active = true},
                    new SCM(){SCMName= SCMs.Fa.ToString(),Active = true},
                    new SCM(){SCMName= SCMs.FiveAnketDeposu.ToString(),Active = true},
                    new SCM(){SCMName= SCMs.DevmoxveAnkethane.ToString(), Active = true},
                    new SCM(){SCMName= SCMs.Kadraj.ToString(), Active = true},
                    new SCM(){SCMName= SCMs.Khalkedon.ToString(), Active = true},
                    new SCM(){SCMName= SCMs.Kolektif.ToString(), Active = true},
                    new SCM(){SCMName= SCMs.Nabi.ToString(), Active = true},
                    new SCM(){SCMName= SCMs.RemindveDemmy.ToString(), Active = true},
                    new SCM(){SCMName= SCMs.SurveyvePola.ToString(), Active = true}, 
                    new SCM(){SCMName= SCMs.Diger.ToString(), Active = true}
                };
                await dbContext.SCM.AddRangeAsync(scms);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
