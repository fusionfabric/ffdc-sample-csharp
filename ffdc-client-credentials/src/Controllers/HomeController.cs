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
        /// This action generates JWT token using oAuth2 client credential flow and consumes the referenital data API using the token
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            string token = _ffdcClientService.GenerateToken();
            if (token != null)
            {
                HttpContext.Session.SetString("token", token);
            }

            return View();
        }

        /// <summary>
        /// This action calls the referential data api to get country list and returns the result as json
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public IActionResult GetCountryDetails()
        {
            string token = HttpContext.Session.GetString("token");

            var result = _ffdcClientService.GetCountries(token);
            return Json(new { data = result.countries });
        }
    }
}