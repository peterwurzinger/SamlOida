using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SamlOida.Web.Controllers
{
    public class HomeController : Controller
    {
        public ContentResult Index()
        {
            return Content($"Hi :-) <a href='{Url.Action("Secret")}'>You shall pass</a>", "text/html");
        }

        [Authorize]
        public ContentResult Secret()
        {
            return Content($"Welcome {User.Identity.Name}! This is a secret message. If you can read this, you're successfully authenticated! :-)<br/><form action='{Url.Action("Logout")}' method='post'><input type='submit' value='Logout'/></form>", "text/html");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Content("Logout in Progress...");
        }

    }
}
