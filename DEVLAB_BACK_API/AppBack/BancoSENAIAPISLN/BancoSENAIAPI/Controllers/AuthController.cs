using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
