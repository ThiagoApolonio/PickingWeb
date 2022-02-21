using Microsoft.AspNet.Identity.Owin;
using Picking_Web.Helpers;
using Picking_Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Picking_Web.Controllers
{
    public class UsuariosController : MyController
    {
        private ApplicationDbContext _context;

        public UsuariosController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Usuarios
        public ActionResult Index()
        {
            var viewModel = new RegisterViewModel()
            {
                Ativo = true,
                Titulo = "Novo Usuário",
                UsuariosSAP = new List<UsuarioSAP>() { },
                Empresas = HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>().Empresa.ToList(),
                QuantidadeLicencas = GlobalHelper.RetornaQuantidadeLicencasDisponiveis(HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>())
            };

            SetUserIdInViewBag();

            return View(viewModel);
        }

        public ActionResult Novo(string x)
        {
            return RedirectToAction("Register", "Account");
        }

        public ActionResult Editar(string id)
        {
            return RedirectToAction("Editar", "Account", new { id = id });
        }
    }
}