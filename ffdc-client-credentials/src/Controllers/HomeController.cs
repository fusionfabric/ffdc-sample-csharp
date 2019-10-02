using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using ffdc_client_credentials.Services;


namespace ffdc_client_credentials.Controllers
{
    public class HomeController : Controller
    {
        private FFDCClientService _ffdcClientService;

        public HomeController(FFDCClientService ffdcClientService)
        {
            _ffdcClientService = ffdcClientService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewBag.isStrong = _ffdcClientService.GetIsStrongValue();
            return View();
        }
    }
}