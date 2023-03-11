using BookShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [AllowAnonymous]
        public IActionResult Index(string id)
        {
            // var UserInfo = User;
            ////string UserId=User.FindFirstValue(ClaimTypes.NameIdentifier);
            // string Role = User.FindFirstValue(ClaimTypes.Role);
            
           
            if (id != null)
                ViewBag.ConfirmEmailAlert = "لینک فعال ساز حساب کاربری به ایمیل شما ارسال شد لطفا با کیلک رو این لینک حساب خود را فعال کنید";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}