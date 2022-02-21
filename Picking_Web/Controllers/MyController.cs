using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace Picking_Web.Controllers
{
    public class MyController : Controller
    {
        protected void SetUserIdInViewBag()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.userid = userid;
        }
    }
}