using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ffdc_client_credentials.Services;
using Microsoft.AspNetCore.Http;

namespace ffdc_.Controllers
{
    public class LoginController : Controller
    {
        private FFDCClientService _ffdcClientService;

        public LoginController(FFDCClientService ffdcClientService)
        {
           
            _ffdcClientService = ffdcClientService;
        }

        [Route("login")]
        public IActionResult Index()
        {
            var isStrong = _ffdcClientService.GetIsStrongValue();
            ViewBag.isStrong = isStrong;
            string token = _ffdcClientService.GenerateToken(isStrong);
            if (!string.IsNullOrEmpty(token))
            {
                HttpContext.Session.SetString("token", token);
                return View("auth", token);
            }

            return View("error", "Unauthorized!");
          
        }

        [Route("logout")]
        public IActionResult logOut()
        {
            ViewBag.isStrong = _ffdcClientService.GetIsStrongValue();
            HttpContext.Session.Remove("token");
            return View("logout");
        }

        public IActionResult Error(string message)
        {
            return View("error", message);
        }
    }
}