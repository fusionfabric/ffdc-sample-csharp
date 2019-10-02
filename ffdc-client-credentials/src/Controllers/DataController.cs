using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using ffdc_client_credentials.Services;

namespace ffdc_client_credentials.Controllers
{
    public class DataController : Controller
    {

        private FFDCClientService _ffdcClientService;

        public DataController(FFDCClientService ffdcClientService) 
        {
            _ffdcClientService = ffdcClientService;
        }

        [Route("results")]
        public IActionResult Index()
        {
            var isStrong= _ffdcClientService.GetIsStrongValue();
            ViewBag.isStrong = isStrong;
            int responseCode = 200;
            string token = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(token))
            {
                return View("~/Views/Login/error.cshtml", "Unauthorized!");
            }
            var result = _ffdcClientService.GetCountries(token, out responseCode);
            if (responseCode == 200)
                return View("results", result);
            else
            {
                return View("~/Views/Login/error.cshtml", "Unauthorized!");
            }
        }
    }
}