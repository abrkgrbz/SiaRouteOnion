using Application.Enums;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Context;

namespace Identity.Seeds
{
    public class DefaultDepartment
    {
        public static async Task SeedAsync(IdentityContext dbContext)
        {
            if (!dbContext.Departments.Any())
            {
                var departments = new List<Department> {
                    new Department{DepartmentName=Departments.InformationTechnology.ToString()},
                    new Department{DepartmentName=Departments.DPAndAnalysis.ToString()},
                    new Department{DepartmentName=Departments.KodlamaSorumlusu.ToString()},
                    new Department{DepartmentName=Departments.Quantitative.ToString()},
                    new Department{DepartmentName=Departments.Partner.ToString()},
                    new Department{DepartmentName=Departments.Research.ToString()},
                    new Department{DepartmentName=Departments.Fieldwork.ToString()},
                    new Department{DepartmentName=Departments.Operations.ToString()},
                    new Department{DepartmentName=Departments.PurchasingDepartment.ToString()},
                    new Department{DepartmentName=Departments.MarketingScienceAndModelling.ToString()} 
                };
                await dbContext.Departments.AddRangeAsync(departments);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
