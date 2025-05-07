using Application.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Context;
using Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Seeds
{
    public static class DefaultSuperAdmin
    {
        public static async Task SeedAsync(IdentityContext dbContext,UserManager<SiaRouteUser> userManager, RoleManager<IdentityRole> roleManager)
        { 
            var departments = await dbContext.Departments.ToListAsync();
            var userData =
               new List<(string UserName, string Email, string Password, string FirstName, string LastName,string DepartmanName )>
               {
                    ("anil.gurbuz", "Anil.Gurbuz@sia-insight.com", "A.bg010203", "Anil Berk", "Gurbuz",Departments.InformationTechnology.ToString()),
                    ("hasan.tolun", "hasan.tolun@sia-insight.com", "nsOp_47nT24", "Hasan", "Tolun",Departments.Operations.ToString()),
                    ("mithat.olut", "mithat.olut@sia-insight.com", "gyIn_19tO33", "Mithat", "Olut",Departments.InformationTechnology.ToString()),
                    ("mustafa.arslanhan", "mustafa.arslanhan@sia-insight.com", "rkTr_57aA34", "Mustafa", "Arslanhan",Departments.Fieldwork.ToString())
               };

            foreach (var (UserName, Email, Password, FirstName, LastName,DepartmanName) in userData)
            {
                var department = departments.FirstOrDefault(d => d.DepartmentName == DepartmanName); 
                var superAdminUser = new SiaRouteUser
                {
                    UserName = UserName,
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    DepartmentId = department?.Id ?? -1 
                };
                if (userManager.Users.All(u => u.Id != superAdminUser.Id))
                {
                    var user = await userManager.FindByEmailAsync(superAdminUser.Email);
                    if (user == null)
                    {
                        await userManager.CreateAsync(superAdminUser, Password);
                        await userManager.AddToRoleAsync(superAdminUser, Roles.Basic.ToString());
                        await userManager.AddToRoleAsync(superAdminUser, Roles.Moderator.ToString());
                        await userManager.AddToRoleAsync(superAdminUser, Roles.Admin.ToString());
                        await userManager.AddToRoleAsync(superAdminUser, Roles.SuperAdmin.ToString());
                    }

                }
            }
        }
    }
}
