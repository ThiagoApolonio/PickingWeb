using Picking_Web.Models;
using System.Web.Mvc;

namespace Picking_Web.Controllers
{
    public class AdministracaoController : MyController
    {
        // GET: Administracao
        [Authorize(Roles = Privilegios.PodeGerenciarAdmin)]
        public ActionResult Index()
        {
            SetUserIdInViewBag();
            return View();
        }
    }
}