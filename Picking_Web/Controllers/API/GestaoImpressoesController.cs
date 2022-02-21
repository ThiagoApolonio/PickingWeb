using Picking_Web.Helpers;
using Picking_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Management;
using System.Web.Http;

namespace Picking_Web.Controllers.API
{
    public class GestaoImpressoesController : ApiController
    {
        private ApplicationDbContext _context;

        public GestaoImpressoesController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetGestaoImpressoras()
        {
            var gestao_impressoras_query = _context.GestaoImpressoes.Include(g => g.Empresa);
            var gestao_impressoras = gestao_impressoras_query.ToList();


            List<object> res = new List<object>() { };
            for (int i = 0; i < gestao_impressoras.Count; i++)
            {
                res.Add(new
                {
                    Id = gestao_impressoras[i].Id,
                    Empresa = gestao_impressoras[i].Empresa.Nome,
                    LugarImpressao = GlobalHelper.LugaresParaImpressao[gestao_impressoras[i].LugarImpressaoId],
                    NomeImpressora = gestao_impressoras[i].NomeImpressora,
                });
            }

            return Ok(res);
        }


        [HttpDelete]
        public IHttpActionResult DeletarGestaoImpressoras(int id)
        {
            var gestaoInDb = _context.GestaoImpressoes.Single(i => i.Id == id);

            if (gestaoInDb == null)
            {
                return NotFound();
            }

            _context.GestaoImpressoes.Remove(gestaoInDb);
            _context.SaveChanges();

            return Ok();
        }


        [HttpPut]
        public IHttpActionResult TestarImpressao(int id)
        {
            GestaoImpressoes gestao = _context.GestaoImpressoes.Single(i => i.Id == id);

            if (gestao == null)
            {
                return NotFound();
            }

            try
            {
                bool achou = false;
                System.Management.ObjectQuery oquery =
                    new System.Management.ObjectQuery("SELECT * FROM Win32_Printer WHERE Name = '" + gestao.NomeImpressora + "' ");

                System.Management.ManagementObjectSearcher mosearcher =
                    new System.Management.ManagementObjectSearcher(oquery);

                System.Management.ManagementObjectCollection moc = mosearcher.Get();

                foreach (ManagementObject mo in moc)
                {
                    if (mo["Name"].ToString() == gestao.NomeImpressora)
                    {
                        achou = true;
                        PrintDocument print = new PrintDocument();
                        print.PrinterSettings.PrinterName = gestao.NomeImpressora;
                        print.Print();

                        break;
                    }
                }
                if (!achou)
                {
                    return BadRequest("Impressora " + gestao.NomeImpressora + " não encontrada");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}
