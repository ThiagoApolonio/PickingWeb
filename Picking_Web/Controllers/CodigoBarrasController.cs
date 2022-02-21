using Picking_Web.Helpers;
using Picking_Web.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;

namespace Picking_Web.Controllers
{
    /// <summary>
    /// Classe para implementação da CONFERENCIA de código de barras
    /// </summary>
    public class CodigoBarrasController : MyController
    {

        private ApplicationDbContext _context;

        public CodigoBarrasController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult Index(int? numdoc, string operador = "")
        {
            string userid = User.Identity.GetUserId();
            int empresa_id = _context.Users.Single(x => x.Id == userid).EmpresaId;

            IOrderedEnumerable<ApplicationUser> operadores = GlobalHelper.GetOperadoresEnumerable(_context, empresa_id, Privilegios.PodeConferirCodigoBarrasID);
            ViewBag.Operadores = operadores;
            ViewBag.empresa = empresa_id;
            ViewBag.numdoc = numdoc;
            ViewBag.operador = operador;

            SetUserIdInViewBag();
            return View("Conferencia");
        }
    }
}