using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Enums;
using Identity.Context;
using Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Seeds
{
    public static class DefaultBasicUser
    {
        public static async Task SeedAsync(IdentityContext dbContext,UserManager<SiaRouteUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var departments = await dbContext.Departments.ToListAsync(); 
            var userData = new List<(string UserName, string Email, string Password, string FirstName, string LastName, string Role,string DepartmanName )> {
                ("aykut.erce", "aykut.erce@sia-insight.com", "veKo_13tE5", "Aykut", "Erce", Roles.Basic.ToString(),Departments.KodlamaSorumlusu.ToString()),
                ("bahar.kurt", "bahar.kurt@sia-insight.com", "veKe_36tT6", "Bahar", "Kurt", Roles.Basic.ToString(), Departments.Quantitative.ToString()),
                ("basak.biyikli", "basak.biyikli@sia-insight.com", "erPa_36kB8", "Basak", "Biyikli", Roles.Basic.ToString(), Departments.Partner.ToString()),
                ("bora.basaran", "bora.basaran@sia-insight.com", "veJu_73aB10", "Bora", "Basaran", Roles.Basic.ToString(), Departments.Quantitative.ToString()),
                ("bora.basaran", "bora.basaran@sia-insight.com", "veJu_73aB10", "Bora", "Basaran", Roles.Basic.ToString(), Departments.Quantitative.ToString()),
                ("deniz.odabasi", "deniz.odabasi@sia-insight.com", "veTe_24zO12", "Deniz", "Odabasi", Roles.Basic.ToString(), Departments.Quantitative.ToString()),
                ("didem.okumusoglu", "didem.okumusoglu@sia-insight.com", "erPa_52mO13", "Didem", "Okumusoglu", Roles.Basic.ToString(), Departments.Partner.ToString()),
                ("ebru.yuksel", "ebru.yuksel@sia-insight.com", "veSe_97uA14", "Ebru", "Yuksel", Roles.Basic.ToString(), Departments.Quantitative.ToString()),
                ("ece.emek", "ece.emek@sia-insight.com", "chRe_67eE15", "Ece", "Emek", Roles.Basic.ToString(), Departments.Research.ToString()),
                ("enes.tasan", "enes.tasan@sia-insight.com", "veJu_23dT16", "Enes", "Tasan", Roles.Basic.ToString(), Departments.Quantitative.ToString()),
                ("furkan.sener", "furkan.sener@sia-insight.com", "ntPr_66nŞ21", "Furkan", "Sener", Roles.Basic.ToString(), Departments.PurchasingDepartment.ToString()),
                ("huseyin.tapinc", "huseyin.tapinc@sia-insight.com", "ntMa_28nT26", "Huseyin", "Tapinc", Roles.Basic.ToString(), Departments.Partner.ToString()),
                ("melis.yilmaz", "melis.yilmaz@sia-insight.com", "vePr_87aY29", "Melis", "Yılmaz", Roles.Basic.ToString(), Departments.Quantitative.ToString()),
                ("meltem.egin", "meltem.egin@sia-insight.com", "veKe_40mE30", "Meltem", "Egin", Roles.Basic.ToString(), Departments.Quantitative.ToString()),
                ("meric.baysal", "meric.baysal@sia-insight.com", "vePr_53çB31", "Meric", "Baysal", Roles.Basic.ToString(), Departments.Quantitative.ToString()),
                ("nurdan.pak", "nurdan.pak@sia-insight.com", "erPa_35nP36", "Nurdan", "Pak", Roles.Basic.ToString(), Departments.KodlamaSorumlusu.ToString()),
                ("ozlem.karatas", "ozlem.karatas@sia-insight.com", "veKe_39mK38", "Ozlem", "Karatas", Roles.Basic.ToString(), Departments.Partner.ToString()),
                ("sila.kamci", "sila.kamci@sia-insight.com", "vePr_70nK39", "Sila", "Kamci", Roles.Basic.ToString(), Departments.Quantitative.ToString()),
                ("sinan.baltali", "sinan.baltali@sia-insight.com", "veRe_29nB40", "Sinan", "Baltali", Roles.Basic.ToString(), Departments.Quantitative.ToString()),
                ("veysel.begtas", "veysel.begtas@sia-insight.com", "ngMa_48lB43", "Veysel", "Begtas", Roles.Basic.ToString(), Departments.MarketingScienceAndModelling.ToString()),
                ("veysel.kamci", "veysel.kamci@sia-insight.com", "ngMa_50lK44", "Veysel", "Kamci", Roles.Basic.ToString(), Departments.MarketingScienceAndModelling.ToString()),
                ("yagmur.akcinar", "yagmur.akcinar@sia-insight.com", "vePr_34rA45", "Yagmur", "Akcinar", Roles.Basic.ToString(), Departments.Quantitative.ToString()),
                ("zeynep.guven", "zeynep.guven@sia-insight.com", "veSe_57pG47", "Zeynep", "Guven", Roles.Basic.ToString(), Departments.Quantitative.ToString()),
            };


            foreach (var (UserName, Email, Password, FirstName, LastName, Role,DepartmanName ) in userData)
            {
                var department = departments.FirstOrDefault(d => d.DepartmentName == DepartmanName); 
                var defaultUser = new SiaRouteUser
                {
                    UserName = UserName,
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    DepartmentId = department?.Id ?? -1 
                };
                if (userManager.Users.All(u => u.Id != defaultUser.Id))
                {
                    var user = await userManager.FindByEmailAsync(defaultUser.Email);
                    if (user == null)
                    {
                        await userManager.CreateAsync(defaultUser, Password);
                        await userManager.AddToRoleAsync(defaultUser, Role);
                    }

                }
            }
        }
    }
}
