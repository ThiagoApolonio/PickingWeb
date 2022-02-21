using Picking_Web.Helpers;
using Picking_Web.Models;
using Picking_Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Web.Mvc;

namespace Picking_Web.Controllers
{
    public class GestaoEtiquetasController : MyController
    {
        private ApplicationDbContext _context;

        public GestaoEtiquetasController()
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
            var viewModel = new FormGestaoEtiquetasViewModel()
            {
                Empresas = _context.Empresa.ToList(),
                Impressoras = ListaDeImpressorasDaMaquina(),
                Users = _context.Users.Where(x => x.Id == "-1").ToList(),
            };
            SetUserIdInViewBag();
            return View("FormGestaoEtiquetas", viewModel);
        }

        public ActionResult Editar(int id)
        {
            var gestao_etiquetas = _context.GestaoEtiquetas.SingleOrDefault(i => i.Id == id);

            if (gestao_etiquetas == null)
            {
                return HttpNotFound();
            }

            var viewModel = new FormGestaoEtiquetasViewModel()
            {
                Id = gestao_etiquetas.Id,
                Empresas = _context.Empresa.ToList(),
                EmpresaId = gestao_etiquetas.EmpresaId,
                UserId = gestao_etiquetas.UserId,
                Users = GlobalHelper.GetUsuarios(_context, gestao_etiquetas.EmpresaId),
                NomeImpressoraEtiqueta = gestao_etiquetas.NomeImpressoraEtiqueta,
                Impressoras = ListaDeImpressorasDaMaquina(),
                IP = gestao_etiquetas.IP,
                Porta = gestao_etiquetas.Porta,
            };

            SetUserIdInViewBag();
            return View("FormGestaoEtiquetas", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Salvar(GestaoEtiquetas gestao_etiquetas)
        {
            // só validar quando for insert
            if (gestao_etiquetas.Id == 0 && GestaoJaExiste(gestao_etiquetas))
            {
                ModelState.AddModelError("EmpresaId", $"Já existe uma configuração de etiqueta para esta empresa, para este usuário.");
            }

            if (!ModelState.IsValid)
            {
                var viewModel = new FormGestaoEtiquetasViewModel()
                {
                    Id = gestao_etiquetas.Id,
                    Empresas = _context.Empresa.ToList(),
                    EmpresaId = gestao_etiquetas.EmpresaId,
                    UserId = gestao_etiquetas.UserId,
                    Users = _context.Users.OrderBy(x => x.UserName).ToList(),
                    NomeImpressoraEtiqueta = gestao_etiquetas.NomeImpressoraEtiqueta,
                    Impressoras = ListaDeImpressorasDaMaquina(),
                    IP = gestao_etiquetas.IP,
                    Porta = gestao_etiquetas.Porta,
                };
                SetUserIdInViewBag();
                return View("FormGestaoEtiquetas", viewModel);
            }

            if (gestao_etiquetas.Id == 0)
            {
                _context.GestaoEtiquetas.Add(gestao_etiquetas);
            }
            else
            {
                var gestaoInDb = _context.GestaoEtiquetas.Single(i => i.Id == gestao_etiquetas.Id);

                gestaoInDb.NomeImpressoraEtiqueta = gestao_etiquetas.NomeImpressoraEtiqueta;
                gestaoInDb.EmpresaId = gestao_etiquetas.EmpresaId;
                gestaoInDb.UserId = gestao_etiquetas.UserId;
                gestaoInDb.IP = gestao_etiquetas.IP;
                gestaoInDb.Porta = gestao_etiquetas.Porta;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "GestaoEtiquetas");
        }

        private bool GestaoJaExiste(GestaoEtiquetas gestao_etiquetas)
        {
            var gestaoInDb = _context.GestaoEtiquetas.SingleOrDefault(i => i.EmpresaId == gestao_etiquetas.EmpresaId && i.UserId == gestao_etiquetas.UserId);

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