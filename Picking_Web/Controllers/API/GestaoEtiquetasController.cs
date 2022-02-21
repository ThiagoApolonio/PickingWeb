using Picking_Web.Helpers;
using Picking_Web.Models;
using System;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Management;
using System.Web.Http;

namespace Picking_Web.Controllers.API
{
    public class GestaoEtiquetasController : ApiController
    {
        private ApplicationDbContext _context;

        public GestaoEtiquetasController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetGestaoEtiquetas()
        {
            _context.GestaoEtiquetas.Include(x => x.Empresa);
            var gestao_etiquetas_query = _context.GestaoEtiquetas.Include(g => g.Empresa).Include(g => g.User);
            var gestao_etiquetas = gestao_etiquetas_query.ToList();

            /*
            List<object> res = new List<object>() { };
            for (int i = 0; i < gestao_etiquetas.Count; i++)
            {
                res.Add(new
                {
                    Id = gestao_etiquetas[i].Id,
                    Empresa = gestao_etiquetas[i].Empresa.Nome,
                    LugarImpressao = GlobalHelper.LugaresParaImpressao[gestao_etiquetas[i].LugarImpressaoId],
                    NomeImpressora = gestao_etiquetas[i].NomeImpressoraEtiqueta,
                });
            }*/

            return Ok(gestao_etiquetas);
        }


        [HttpDelete]
        public IHttpActionResult DeletarGestaoEtiquetas(int id)
        {
            var gestaoInDb = _context.GestaoEtiquetas.Single(i => i.Id == id);

            if (gestaoInDb == null)
            {
                return NotFound();
            }

            _context.GestaoEtiquetas.Remove(gestaoInDb);
            _context.SaveChanges();

            return Ok();
        }


        [HttpPut]
        public IHttpActionResult TestarImpressao(int id)
        {
            GestaoEtiquetas gestao = _context.GestaoEtiquetas.Single(i => i.Id == id);

            if (gestao == null)
            {
                return NotFound();
            }

            try
            {
                bool achou = false;
                System.Management.ObjectQuery oquery =
                    new System.Management.ObjectQuery("SELECT * FROM Win32_Printer WHERE Name = '" + gestao.NomeImpressoraEtiqueta + "' ");

                System.Management.ManagementObjectSearcher mosearcher =
                    new System.Management.ManagementObjectSearcher(oquery);

                System.Management.ManagementObjectCollection moc = mosearcher.Get();

                foreach (ManagementObject mo in moc)
                {
                    if (mo["Name"].ToString() == gestao.NomeImpressoraEtiqueta)
                    {
                        achou = true;
                        PrintDocument print = new PrintDocument();
                        print.PrinterSettings.PrinterName = gestao.NomeImpressoraEtiqueta;
                        print.Print();

                        break;
                    }
                }
                if (!achou)
                {
                    return BadRequest("Impressora " + gestao.NomeImpressoraEtiqueta + " não encontrada");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        // GET /api/usuarios
        public IHttpActionResult GetUsuarios(int empresa_id)
        {
            try
            {
                var users = GlobalHelper.GetUsuarios(_context, empresa_id);
                return Ok(users);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
