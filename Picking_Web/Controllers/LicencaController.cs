using Picking_Web.Helpers;
using Picking_Web.Models;
using Picking_Web.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace Picking_Web.Controllers
{
    public class LicencaController : MyController
    {

        private ApplicationDbContext _context;

        public LicencaController()
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

        public ActionResult Editar(int id)
        {
            var licenca = _context.Licenca.SingleOrDefault(i => i.Id == id);

            if (licenca == null)
            {
                return HttpNotFound();
            }

            var viewModel = new FormLicencaViewModel()
            {
                Id = licenca.Id,
                QuantidadeLicenca = int.Parse(EncryptionHelper.Base64Decode(licenca.QuantidadeLicencas))
            };
            SetUserIdInViewBag();
            return View("FormLicenca", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Salvar(FormLicencaViewModel form_licenca)
        {
            var qtd_usuarios_licenciados = GlobalHelper.RetornaQuantidadeLicencasUtilizando(_context);

            if (qtd_usuarios_licenciados > form_licenca.QuantidadeLicenca)
            {
                ModelState.AddModelError("QuantidadeLicenca", $"Existem {qtd_usuarios_licenciados} usuário marcados como licenciados. Remova a licença dos mesmos.");
            }

            if (!ModelState.IsValid)
            {
                SetUserIdInViewBag();
                return View("FormLicenca", form_licenca);
            }

            if (form_licenca.Id > 0)
            {
                var licencaInDB = _context.Licenca.Single(i => i.Id == form_licenca.Id);

                licencaInDB.QuantidadeLicencas = EncryptionHelper.Base64Encode(form_licenca.QuantidadeLicenca.ToString());
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Licenca");
        }

    }
}