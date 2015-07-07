using System.Web.Mvc;

namespace Triage.UI.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public ContentResult Index()
        {
            return new ContentResult {Content = "Welcome"};
        } 
    }
}