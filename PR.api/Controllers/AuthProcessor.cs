using Microsoft.AspNetCore.Mvc;

namespace PR.api.Controllers
{
    public class AuthProcessor : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
