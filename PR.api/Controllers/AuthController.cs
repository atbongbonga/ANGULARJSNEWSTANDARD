using Microsoft.AspNetCore.Mvc;

namespace PR.api.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
