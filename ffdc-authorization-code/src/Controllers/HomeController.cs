using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ffdc_authorization_code.Services;

namespace ffdc_authorization_code.Controllers
{
    public class HomeController : Controller
    {
        private FFDCClientService _ffdcClientService;
        public HomeController(FFDCClientService ffdcClientService)
        {
            _ffdcClientService = ffdcClientService;
        }
        public IActionResult Index()
        {
            ViewBag.isStrong = _ffdcClientService.GetIsStrongValue();
            return View();
        }
    }
}