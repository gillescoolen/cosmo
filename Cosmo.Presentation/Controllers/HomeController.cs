using Microsoft.AspNetCore.Mvc;

namespace Cosmo.Presentation.Controllers
{
  public class HomeController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}