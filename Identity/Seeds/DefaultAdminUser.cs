using Application.Enums;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Context;

namespace Identity.Seeds
{
    public class DefaultAdminUser
    {
        public static async Task SeedAsync(IdentityContext dbContext,UserManager<SiaRouteUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var departments = await dbContext.Departments.ToListAsync(); 
            var userData =
                new List<(string UserName, string Email, string Password, string FirstName, string LastName,string DepartmanName )>
                {
                    ("aydin.degirmenci", "aydin.degirmenci@sia-insight.com", "isTr_23nD4", "Aydin", "Degirmenci",Departments.DPAndAnalysis.ToString()),
                    ("eray.girgin", "eray.girgin@sia-insight.com", "rkOn_74yG17", "Eray", "Girgin", Departments.DPAndAnalysis.ToString()),
                    ("ezgi.boztepe", "ezgi.boztepe@sia-insight.com", "nsOn_80iB19", "Ezgi", "Boztepe", Departments.Operations.ToString()),
                    ("firdevs.tokgoz", "firdevs.tokgoz@sia-insight.com", "rkOn_70iT20", "Firdevs", "Tokgoz", Departments.Fieldwork.ToString()),
                    ("gamze.erener", "gamze.erener@sia-insight.com", "rkTr_98eE22", "Gamze", "Erener", Departments.Fieldwork.ToString()), 
                    ("hakan.turan", "hakan.turan@sia-insight.com", "isOn_90nT23", "Hakan", "Turan", Departments.DPAndAnalysis.ToString()),
                    ("nurdan.didin", "nurdan.didin@sia-insight.com", "isDP_16nD35", "Nurdan", "Didin", Departments.DPAndAnalysis.ToString()),
                    ("onur.arslan", "onur.arslan@sia-insight.com", "isTr_98rA37", "Onur", "Arslan", Departments.DPAndAnalysis.ToString()),
                    ("sinem.eser", "sinem.eser@sia-insight.com", "isOn_23mE41", "Sinem", "Eser", Departments.DPAndAnalysis.ToString()), 
                    ("eda.tuncay", "eda.tuncay@sia-insight.com", "Tued_281A48", "Eda", "Tuncay", Departments.DPAndAnalysis.ToString()), 
                    ("kaan.tuncay", "kaan.tuncay@sia-insight", "Tuka_771A49", "Kaan", "Tuncay", Departments.DPAndAnalysis.ToString()),
                };
             
            foreach (var (UserName, Email, Password, FirstName, LastName, DepartmanName ) in userData)
            {
                var department = departments.FirstOrDefault(d => d.DepartmentName == DepartmanName); 
                var adminUser = new SiaRouteUser
                {
                    UserName = UserName,
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    DepartmentId = department?.Id ?? -1
                };
                if (userManager.Users.All(u => u.Id != adminUser.Id))
                {
                    var user = await userManager.FindByEmailAsync(adminUser.Email);
                    if (user == null)
                    {
                        await userManager.CreateAsync(adminUser, Password);
                        await userManager.AddToRoleAsync(adminUser, Roles.Basic.ToString());
                        await userManager.AddToRoleAsync(adminUser, Roles.Moderator.ToString());
                        await userManager.AddToRoleAsync(adminUser, Roles.Admin.ToString());
                    }

                }
            }
        }
    }
}
