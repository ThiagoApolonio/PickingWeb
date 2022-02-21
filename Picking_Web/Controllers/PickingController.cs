using Picking_Web.Helpers;
using Picking_Web.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;

namespace Picking_Web.Controllers
{
    public class PickingController : MyController
    {
        private ApplicationDbContext _context;

        public PickingController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Authorize]
        public ActionResult Index()
        {
            string userid = User.Identity.GetUserId();
            int empresa_id = _context.Users.Single(x => x.Id == userid).EmpresaId;
            ViewBag.NomeImpressora = GetDefaultPrintername(empresa_id);
            ViewBag.empresa = empresa_id;

            SetUserIdInViewBag();

            if (string.IsNullOrEmpty(ViewBag.NomeImpressora))
            {
                return View("ErrorImpressoraNaoEncontrada");
            }
            return View("ListaPicking");
        }

        [Authorize]
        public ActionResult Acompanhamento()
        {
            string userid = User.Identity.GetUserId();
            int empresa_id = _context.Users.Single(x => x.Id == userid).EmpresaId;
            Empresa empresa = _context.Empresa.Single(x => x.Id == empresa_id);
            ViewBag.timer = empresa.Timer;
            ViewBag.NomeImpressora = GetDefaultPrintername(empresa_id);
            ViewBag.empresa = empresa_id;

            SetUserIdInViewBag();

            if (string.IsNullOrEmpty(ViewBag.NomeImpressora))
            {
                return View("ErrorImpressoraNaoEncontrada");
            }
            return View("AcompanhamentoPicking");
        }

        public string GetDefaultPrintername(int empresa_id)
        {

            var gestao_impressoras = _context.GestaoImpressoes.SingleOrDefault(i => i.EmpresaId == empresa_id && i.LugarImpressaoId == GlobalHelper.LugarParaImprimirGeracaoListaPicking);

            if (gestao_impressoras == null)
            {
                return "";
            }

            return gestao_impressoras.NomeImpressora;
        }
    }
}