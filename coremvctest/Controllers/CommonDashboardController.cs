using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace coremvctest.Controllers
{
    public class CommonDashboardController : Controller
    {
        public IActionResult CommonDashboardView()
        {
            return View();
        }
        public async Task<IActionResult> LogOut()
        {
            // Clear user session data
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to CommonDashboardView
            return RedirectToAction("CommonDashboardView");
        }
    }
}
