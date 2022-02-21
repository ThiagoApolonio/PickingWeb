using Microsoft.AspNet.Identity;
using Picking_Web.Helpers;
using Picking_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Picking_Web.Controllers
{
    public class BaixaController : MyController
    {
        private ApplicationDbContext _context;

        public BaixaController()
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

            return View("Baixa");
        }

       
    }
}