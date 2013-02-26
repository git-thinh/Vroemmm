using System.Web.Mvc;

namespace Vroemmm.Controllers
{
    public partial class HomeController : Controller
    {
        public virtual ActionResult Index()
        {
            return View();
        }      
    }
}
