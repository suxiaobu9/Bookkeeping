using Microsoft.AspNetCore.Mvc;

namespace Bookkeeping.Controllers
{
    /// <summary>
    /// ngrok http 5000 -host-header="localhost:5000"
    /// </summary>
    public class BookkeepingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
