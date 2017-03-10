using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SamlOida.Web.Controllers
{
    public class HomeController : Controller
    {
        public ContentResult Index()
        {
            return Content($"Hi :-) <a href='{Url.Action("Secret")}'>Show me your secret</a>", "text/html");
        }

        [Authorize]
        public ContentResult Secret()
        {
            return Content($"Welcome {User.Identity.Name}! This is a secret message... ;-)");
        }

    }
}
