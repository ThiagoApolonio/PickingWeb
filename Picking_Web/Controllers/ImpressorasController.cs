using Picking_Web.Models;
using Picking_Web.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace Picking_Web.Controllers
{
    public class ImpressorasController : MyController
    {
        private ApplicationDbContext _context;

        public ImpressorasController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult Index()
        {
            SetUserIdInViewBag();
            return View();
        }

        public ActionResult Novo()
        {
            var tipoImpressoras = _context.TipoImpressoras.ToList();
            var viewModel = new FormImpressoraViewModel()
            {
                Impressora = new Impressora()
                {
                    Ativo = true
                },
                TipoImpressoras = tipoImpressoras,
            };
            SetUserIdInViewBag();
            return View("FormImpressora", viewModel);
        }

        public ActionResult Editar(int id)
        {
            var impressora = _context.Impressoras.SingleOrDefault(i => i.Id == id);

            if (impressora == null)
            {
                return HttpNotFound();
            }

            var viewModel = new FormImpressoraViewModel()
            {
                Impressora = impressora,
                TipoImpressoras = _context.TipoImpressoras.ToList()
            };
            SetUserIdInViewBag();
            return View("FormImpressora", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Salvar(Impressora impressora)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new FormImpressoraViewModel()
                {
                    Impressora = impressora,
                    TipoImpressoras = _context.TipoImpressoras.ToList()
                };
                SetUserIdInViewBag();
                return View("FormImpressora", viewModel);
            }

            if (impressora.Id == 0)
            {
                _context.Impressoras.Add(impressora);
            }
            else
            {
                var impressoraInDb = _context.Impressoras.Single(i => i.Id == impressora.Id);

                impressoraInDb.Ativo = impressora.Ativo;
                impressoraInDb.Descricao = impressora.Descricao;
                impressoraInDb.TipoImpressoraId = impressora.TipoImpressoraId;
                impressoraInDb.IP = impressora.IP;
                impressoraInDb.Porta = impressora.Porta;
                impressoraInDb.Localizacao = impressora.Localizacao;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Impressoras");
        }
    }
}