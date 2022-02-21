using Picking_Web.Helpers;
using Picking_Web.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Picking_Web.Controllers
{
    public class EtiquetasController : MyController
    {
        private ApplicationDbContext _context;

        public EtiquetasController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Etiquetas
        public ActionResult Index()
        {
            string userid = User.Identity.GetUserId();
            int empresa_id = _context.Users.Single(x => x.Id == userid).EmpresaId;

            IOrderedEnumerable<ApplicationUser> operadores = GlobalHelper.GetOperadoresEnumerable(_context, empresa_id, Privilegios.PodeGerenciarEtiquetaID);
            ViewBag.Operadores = operadores;
            ViewBag.empresa = empresa_id;

            Dictionary<string, string> dados = GetGestaoEtiquetasDados(empresa_id);
            ViewBag.NomeImpressora = dados["nome_impressora"];
            ViewBag.IP = dados["ip"];
            ViewBag.Porta = dados["porta"];
            ViewBag.DadosBalancaOK = !string.IsNullOrEmpty(dados["ip"]) && !string.IsNullOrEmpty(dados["porta"]);
            SetUserIdInViewBag();

            if (string.IsNullOrEmpty(ViewBag.NomeImpressora))
            {
                return View("ErrorImpressoraNaoEncontrada");
            }

            return View("Etiquetas");
        }

        public Dictionary<string, string> GetGestaoEtiquetasDados(int empresa_id)
        {
            Dictionary<string, string> dados = new Dictionary<string, string>() {
                { "nome_impressora", ""},
                { "ip", ""},
                { "porta", ""},
            };

            string usuario_logado = User.Identity.GetUserId();
            var gestao_etiquetas = _context.GestaoEtiquetas.SingleOrDefault(i => i.EmpresaId == empresa_id && i.UserId == usuario_logado);

            if (gestao_etiquetas != null)
            {
                dados["nome_impressora"] = gestao_etiquetas.NomeImpressoraEtiqueta;
                dados["ip"] = gestao_etiquetas.IP;
                dados["porta"] = gestao_etiquetas.Porta;
            }

            return dados;
        }
    }
}