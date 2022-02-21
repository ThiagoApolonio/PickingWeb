using Picking_Web.Helpers;
using Picking_Web.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Picking_Web.Controllers
{
    public class RecebimentoController : MyController
    {
        private ApplicationDbContext _context;

        public RecebimentoController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Recebimento
        public ActionResult Index()
        {
            string userid = User.Identity.GetUserId();
            int empresa_id = _context.Users.Single(x => x.Id == userid).EmpresaId;

            List<ItemSAP> itens = SAPHelper.GetItensSap(_context, empresa_id);
            ViewBag.Itens = itens;
            ViewBag.empresa = empresa_id;

            SetUserIdInViewBag();

            ViewBag.NomeImpressoraAmbiente = GetDefaultPrintername(empresa_id, GlobalHelper.LugarParaImprimirImpressoraAmbiente);
            if (string.IsNullOrEmpty(ViewBag.NomeImpressoraAmbiente))
            {
                return View("ErrorImpressoraNaoEncontradaAmbiente");
            }

            ViewBag.NomeImpressoraGeladeira = GetDefaultPrintername(empresa_id, GlobalHelper.LugarParaImprimirImpressoraGeladeira);
            if (string.IsNullOrEmpty(ViewBag.NomeImpressoraGeladeira))
            {
                return View("ErrorImpressoraNaoEncontradaGeladeira");
            }

            return View("Recebimento");
        }
        public ActionResult Reimpressao()
        {
            string userid = User.Identity.GetUserId();
            int empresa_id = _context.Users.Single(x => x.Id == userid).EmpresaId;

            List<ItemSAP> itens = SAPHelper.GetItensSap(_context, empresa_id);
            ViewBag.Itens = itens;
            ViewBag.empresa = empresa_id;

            SetUserIdInViewBag();

            ViewBag.NomeImpressoraAmbiente = GetDefaultPrintername(empresa_id, GlobalHelper.LugarParaImprimirImpressoraAmbiente);
            if (string.IsNullOrEmpty(ViewBag.NomeImpressoraAmbiente))
            {
                return View("ErrorImpressoraNaoEncontradaAmbiente");
            }

            ViewBag.NomeImpressoraGeladeira = GetDefaultPrintername(empresa_id, GlobalHelper.LugarParaImprimirImpressoraGeladeira);
            if (string.IsNullOrEmpty(ViewBag.NomeImpressoraGeladeira))
            {
                return View("ErrorImpressoraNaoEncontradaGeladeira");
            }

            return View("RecebimentoImpressao");
        }


        public string GetDefaultPrintername(int empresa_id, int lugar_para_imprimir)
        {
            var gestao_impressoras = _context.GestaoImpressoes.SingleOrDefault(i => i.EmpresaId == empresa_id && i.LugarImpressaoId == lugar_para_imprimir);

            if (gestao_impressoras == null)
            {
                return "";
            }

            return gestao_impressoras.NomeImpressora;
        }
    }
}