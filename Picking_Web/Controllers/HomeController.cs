using Microsoft.AspNet.Identity;
using Picking_Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using Picking_Web.Helpers;

using System.Linq;


namespace Picking_Web.Controllers
{
    public class HomeController : MyController
    {
        private ApplicationDbContext _context;
        public HomeController()
        {
            _context = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            List<string> controllers = new List<string>();
            string userid = User.Identity.GetUserId();
            int empresa_id = _context.Users.Single(x => x.Id == userid).EmpresaId;
            ViewBag.empresa = empresa_id;
            SetUserIdInViewBag();
            if (User.IsInRole(Privilegios.PodeGerenciarAdmin))
            {
                controllers.Add(Privilegios.PodeGerenciarAdminCONTROLLER);
            }
            if (User.IsInRole(Privilegios.PodeGerenciarListaPicking))
            {
                controllers.Add(Privilegios.PodeGerenciarListaPickingCONTROLLER);
            }
            if (User.IsInRole(Privilegios.PodeConferirCodigoBarras))
            {
                controllers.Add(Privilegios.PodeConferirCodigoBarrasCONTROLLER);
            }
            if (User.IsInRole(Privilegios.PodeGerenciarEtiqueta))
            {
                controllers.Add(Privilegios.PodeGerenciarEtiquetaCONTROLLER);
            }
            if (User.IsInRole(Privilegios.PodeGerenciarRecebimento))
            {
                controllers.Add(Privilegios.PodeGerenciarRecebimentoCONTROLLER);
            }
            if (User.IsInRole(Privilegios.PodeGerenciarDocumentos))
            {
                controllers.Add(Privilegios.PodeGerenciarDocumentosCONTROLLER);
            }
            if (User.IsInRole(Privilegios.PodeGerenciarBaixa))
            {
                controllers.Add(Privilegios.PodeGerenciarBaixaCONTROLLER);
            }
            if (controllers.Count == 1)
            {
                return RedirectToAction("Index", controllers[0]);
            }
            else
            {
                SetUserIdInViewBag();
                return View();
            }
        }


    }
}