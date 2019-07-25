using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using ffdc_authorization_code.Services;

namespace ffdc_authorization_code.Controllers
{
    public class LoginController : Controller
    {
        private FFDCClientService _ffdcClientService;

        public LoginController(FFDCClientService ffdcClientService)
        {
            _ffdcClientService = ffdcClientService;
        }

        public IActionResult Index()
        {
            string authCodeurl = _ffdcClientService.GetFFDCAuthCodeUri();
            return Redirect(authCodeurl);
        }

        [Route("login/oauth2/code/finastra")]
        public IActionResult GenerateAccessToken(string code)
        {
            string token = _ffdcClientService.GenerateToken(code);
            if (!string.IsNullOrEmpty(token))
            {
                HttpContext.Session.SetString("token", token);
                return RedirectToAction("Home");
            }

            return RedirectToAction("Error",new { message = "Error Login,please try again"});
        }

        public IActionResult Home()
        {
            return View("Index");
        }

        public IActionResult Signout()
        {
            HttpContext.Session.Remove("token");
            return  RedirectToAction("", "Home");
        }

        public IActionResult Error(string message)
        {
            return View("error",message);
        }
    }
}