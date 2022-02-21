using Microsoft.AspNet.Identity.Owin;
using Picking_Web.Helpers;
using Picking_Web.Models;
using Picking_Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Picking_Web.Controllers
{
    public class EmpresasController : MyController
    {
        private ApplicationDbContext _context;

        public EmpresasController()
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

        public ActionResult Nova()
        {
            var viewModel = new FormEmpresaViewModel()
            {
                Ativo = true,
                DepositoSAP = new List<DepositosSap>() { }
            };

            ViewBag.Title = "Cadastrar Empresa";
            SetUserIdInViewBag();
            return View("FormEmpresa", viewModel);
        }

        public ActionResult Editar(int id)
        {
            var empresa = _context.Empresa.SingleOrDefault(i => i.Id == id);

            if (empresa == null)
            {
                return HttpNotFound();
            }

            var viewModel = new FormEmpresaViewModel()
            {
                Id = empresa.Id,
                Nome = empresa.Nome,
                Ativo = empresa.Ativo,
                ContadorLote = empresa.ContadorLote,
                Licenciado = empresa.Licenciado,
                NomeBanco = empresa.NomeBanco,
                InstanciaBanco = empresa.InstanciaBanco,
                UsuarioBanco = empresa.UsuarioBanco,
                SenhaBanco = empresa.SenhaBanco,
                TiposBancoId = empresa.TipoBanco,
                LicencaSap = empresa.LicencaSap,
                PortaSap = empresa.PortaSap,
                UsuarioSap = empresa.UsuarioSap,
                SenhaSap = empresa.SenhaSap,
                DepositoSAP = SAPHelper.GetDepositoSap(HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>(), empresa.Id),
                selectedDeposito = string.IsNullOrEmpty(empresa.DepoSapId)?null: empresa.DepoSapId.Split(',').ToArray(),
                DepoPadrao = empresa.DepoPadrao,
                Timer = empresa.Timer,
            };

            ViewBag.Title = "Editar Empresa";
            SetUserIdInViewBag();
            return View("FormEmpresa", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Salvar(FormEmpresaViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                SetUserIdInViewBag();
                return View("FormEmpresa", viewModel);
            }

            if (viewModel.Id == 0)
            {
                Empresa empresa = new Empresa()
                {
                    Nome = viewModel.Nome,
                    Ativo = viewModel.Ativo,
                    ContadorLote = viewModel.ContadorLote,
                    Licenciado = viewModel.Licenciado,

                    LicencaSap = viewModel.LicencaSap,
                    PortaSap = viewModel.PortaSap,
                    UsuarioSap = viewModel.UsuarioSap,
                    SenhaSap = viewModel.SenhaSap,

                    InstanciaBanco = viewModel.InstanciaBanco,
                    NomeBanco = viewModel.NomeBanco,
                    TipoBanco = viewModel.TiposBancoId,
                    UsuarioBanco = viewModel.UsuarioBanco,
                    SenhaBanco = viewModel.SenhaBanco,
                    DepoSapId = viewModel.selectedDeposito is null ? "" : string.Join(",", viewModel.selectedDeposito),
                    DepoPadrao = viewModel.DepoPadrao,
                    Timer = viewModel.Timer
                };

                _context.Empresa.Add(empresa);
            }
            else
            {
                var empresaInDb = _context.Empresa.Single(i => i.Id == viewModel.Id);

                empresaInDb.Nome = viewModel.Nome;
                empresaInDb.Ativo = viewModel.Ativo;
                empresaInDb.ContadorLote = viewModel.ContadorLote;
                empresaInDb.Licenciado = viewModel.Licenciado;

                empresaInDb.LicencaSap = viewModel.LicencaSap;
                empresaInDb.PortaSap = viewModel.PortaSap;
                empresaInDb.UsuarioSap = viewModel.UsuarioSap;
                empresaInDb.SenhaSap = viewModel.SenhaSap;

                empresaInDb.InstanciaBanco = viewModel.InstanciaBanco;
                empresaInDb.NomeBanco = viewModel.NomeBanco;
                empresaInDb.TipoBanco = viewModel.TiposBancoId;
                empresaInDb.UsuarioBanco = viewModel.UsuarioBanco;
                empresaInDb.SenhaBanco = viewModel.SenhaBanco;
                empresaInDb.DepoSapId =viewModel.selectedDeposito is null? "" :string.Join(",", viewModel.selectedDeposito);
                empresaInDb.DepoPadrao = viewModel.DepoPadrao;

                empresaInDb.Timer = viewModel.Timer;
            }

            _context.SaveChanges();



            return RedirectToAction("Index", "Empresas");
        }
    }
}
