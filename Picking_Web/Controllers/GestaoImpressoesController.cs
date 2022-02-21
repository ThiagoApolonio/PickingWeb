using Picking_Web.Models;
using Picking_Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Web.Mvc;

namespace Picking_Web.Controllers
{
    public class GestaoImpressoesController : MyController
    {
        private ApplicationDbContext _context;

        public GestaoImpressoesController()
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
            var viewModel = new FormGestaoImpressoesViewModel()
            {
                Empresas = _context.Empresa.ToList(),
                Impressoras = ListaDeImpressorasDaMaquina()
            };
            SetUserIdInViewBag();
            return View("FormGestaoImpressoes", viewModel);
        }

        public ActionResult Editar(int id)
        {
            var gestao_impressoes = _context.GestaoImpressoes.SingleOrDefault(i => i.Id == id);

            if (gestao_impressoes == null)
            {
                return HttpNotFound();
            }

            var viewModel = new FormGestaoImpressoesViewModel()
            {
                Id = gestao_impressoes.Id,
                Empresas = _context.Empresa.ToList(),
                EmpresaId = gestao_impressoes.EmpresaId,
                LugarImpressaoId = gestao_impressoes.LugarImpressaoId,
                NomeImpressora = gestao_impressoes.NomeImpressora,
                Impressoras = ListaDeImpressorasDaMaquina()
            };
            SetUserIdInViewBag();
            return View("FormGestaoImpressoes", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Salvar(GestaoImpressoes gestao_impressoes)
        {
            // só validar quando for insert
            if (gestao_impressoes.Id == 0 && GestaoJaExiste(gestao_impressoes))
            {
                ModelState.AddModelError("EmpresaId", $"Já existe uma configuração de impressora para esta empresa, para este local de impressão.");
            }

            if (!ModelState.IsValid)
            {
                var viewModel = new FormGestaoImpressoesViewModel()
                {
                    Id = gestao_impressoes.Id,
                    Empresas = _context.Empresa.ToList(),
                    EmpresaId = gestao_impressoes.EmpresaId,
                    LugarImpressaoId = gestao_impressoes.LugarImpressaoId,
                    NomeImpressora = gestao_impressoes.NomeImpressora,
                    Impressoras = ListaDeImpressorasDaMaquina()
                };
                SetUserIdInViewBag();
                return View("FormGestaoImpressoes", viewModel);
            }

            if (gestao_impressoes.Id == 0)
            {
                _context.GestaoImpressoes.Add(gestao_impressoes);
            }
            else
            {
                var gestaoInDb = _context.GestaoImpressoes.Single(i => i.Id == gestao_impressoes.Id);

                gestaoInDb.NomeImpressora = gestao_impressoes.NomeImpressora;
                gestaoInDb.EmpresaId = gestao_impressoes.EmpresaId;
                gestaoInDb.LugarImpressao = gestao_impressoes.LugarImpressao;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "GestaoImpressoes");
        }

        private bool GestaoJaExiste(GestaoImpressoes gestao_impressoes)
        {
            var gestaoInDb = _context.GestaoImpressoes.SingleOrDefault(i => i.EmpresaId == gestao_impressoes.EmpresaId && i.LugarImpressaoId == gestao_impressoes.LugarImpressaoId);

            return gestaoInDb != null;
        }

        public List<object> ListaDeImpressorasDaMaquina()
        {
            List<object> res = new List<object>() { };

            ManagementScope objScope = new ManagementScope(ManagementPath.DefaultPath); //For the local Access
            objScope.Connect();

            SelectQuery selectQuery = new SelectQuery();
            selectQuery.QueryString = "Select * from win32_Printer";
            ManagementObjectSearcher MOS = new ManagementObjectSearcher(objScope, selectQuery);
            ManagementObjectCollection MOC = MOS.Get();
            foreach (ManagementObject mo in MOC)
            {
                string printerName = mo["Name"].ToString();
                res.Add(new
                {
                    Id = printerName,
                    Nome = printerName
                });
            }

            return res;
        }
    }
}