using AutoMapper;
using Picking_Web.DTOs;
using Picking_Web.Helpers;
using Picking_Web.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace Picking_Web.Controllers.API
{
    public class EmpresasController : ApiController
    {

        private ApplicationDbContext _context;

        public EmpresasController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetEmpresas(string query = null)
        {
            var empresas = _context.Empresa.ToList().Select(Mapper.Map<Empresa, EmpresasDto>);

            return Ok(empresas);
        }

        [HttpDelete]
        public IHttpActionResult DeletarEmpresas(int id)
        {
            var empresaInDb = _context.Empresa.Single(i => i.Id == id);

            if (empresaInDb == null)
            {
                return NotFound();
            }

            _context.Empresa.Remove(empresaInDb);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut]
        public IHttpActionResult CriarCampos(int id)
        {
            Empresa empresa = _context.Empresa.Single(i => i.Id == id);

            if (empresa == null)
            {
                return NotFound();
            }

            SAPbobsCOM.Company oCompany = null;
            try
            {
                oCompany = SAPHelper.CriarCompany(empresa);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (oCompany.Connected)
            {
                DataBaseFunctions odbFunc = new DataBaseFunctions(oCompany);

                try
                {
                    odbFunc.ManipulaCamposPicking();
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }

                return Ok();
            }
            else
            {
                string error = oCompany.GetLastErrorDescription();
                int error_code = oCompany.GetLastErrorCode();
                string msg = "Erro ao tentar conectar com o SAP Business One.\nCódigo do Erro: " + error_code + ".\nDescrição: " + error;
                return BadRequest(msg);
            }
        }

        [HttpPut]
        public IHttpActionResult TestarConexao(int id)
        {
            Empresa empresa = _context.Empresa.Single(i => i.Id == id);

            if (empresa == null)
            {
                return NotFound();
            }

            SAPbobsCOM.Company oCompany = null;
            try
            {
                oCompany = SAPHelper.CriarCompany(empresa);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (oCompany.Connected)
            {
                return Ok();
            }
            else
            {
                string error = oCompany.GetLastErrorDescription();
                int error_code = oCompany.GetLastErrorCode();
                string msg = "Erro ao tentar conectar com o SAP Business One.\nCódigo do Erro: " + error_code + ".\nDescrição: " + error;
                return BadRequest(msg);
            }
        }
        

    }
}
