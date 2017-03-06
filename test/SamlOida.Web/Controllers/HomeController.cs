using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace SamlOida.Web.Controllers
{
    public class HomeController : Controller
    {
        public ContentResult Index()
        {
            return Content($"Welcome :-)<a href='signin-saml'>Login</a>", "text/html");
        }

    }
}
