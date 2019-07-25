using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;


namespace ffdc_authorization_code.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }
 
        public IActionResult Index()
        {          
            return View();
        }      
    }
}