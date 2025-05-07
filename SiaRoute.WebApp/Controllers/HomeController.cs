using Microsoft.AspNetCore.Mvc;
using SiaRoute.WebApp.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace SiaRoute.WebApp.Controllers
{
 
    public class HomeController : BaseController
    {
         
        [Authorize]
        public IActionResult Index()
        {
             
            return View();
        }

       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
    }
}

