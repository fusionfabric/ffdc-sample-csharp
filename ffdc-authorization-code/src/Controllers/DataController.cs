using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using ffdc_authorization_code.Services;

namespace ffdc_authorization_code.Controllers
{
    public class DataController : Controller
    {
        private FFDCClientService _ffdcClientService;

        public DataController(FFDCClientService ffdcClientService)
        {
            _ffdcClientService = ffdcClientService;
        }

        [Route("fxspot/trades")]
        public IActionResult Index()
        {
            return View();
        }
        
        /// <summary>
        /// This loads the table with trades by calling FFDC trade capture API by authorization code flow authentication
        /// </summary>
        /// <returns></returns>
        public IActionResult LoadTable()
        {
            string token = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Error", "Login", new { message = "Token should be available" });
            }

            var tradeSummaryList = _ffdcClientService.GetFxSpotTrades(token);

            return Json(new { data = tradeSummaryList.items });
        }
    }
}