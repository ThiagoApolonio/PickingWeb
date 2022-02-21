using Picking_Web.Helpers;
using Picking_Web.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using SAPbobsCOM;

namespace Picking_Web.Controllers
{
    public class DocumentosController : MyController
    {
        private ApplicationDbContext _context;
        // GET: Documentos
        public ActionResult Index()
        {
            return View(new Documentos());
        }

        [HttpPost]
        public ActionResult Salvar(Documentos documentos)
        {
            string status = documentos.Status[0].Id;
            int empresa = documentos.ListaEmpresas[0].Id;
            int CodePckEtapa = GetCodePckEtapa(documentos.NDocumento, empresa);
            RetornaStatusPedido(documentos.NDocumento, status, CodePckEtapa, empresa);
            RetornaStatusPicking(documentos.NDocumento, status, CodePckEtapa, empresa);
            RetornaStatusOperador(documentos.NDocumento, status, CodePckEtapa, empresa);
            if (status == "AS")
                CancelaListaPickingAberto(documentos.NDocumento, empresa);

            return View();
        }


        public int GetCodePckEtapa(string docentry, int empresa_id)
        {
            _context = new ApplicationDbContext();
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
            string connectionString = DBHelper.GetConnectionString(empresa);
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string sql = @"select top 1 Code from [@UPD_PCK_ETAPA] where U_docentry ='" + docentry + "' order by Code desc";
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        command.CommandText = sql;
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            return NumberHelper.GetFromDBToInt(reader["Code"]);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }

        public void RetornaStatusPedido(string docentry, string status, int code, int empresa_id)
        {
            _context = new ApplicationDbContext();
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
            string connectionString = DBHelper.GetConnectionString(empresa);
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string sql = string.Format(@"update ORDR Set U_UPD_PCK_STATUS = '{1}'
                                                    where DocEntry = '{0}'", docentry, status);
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        command.CommandText = sql;
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void RetornaStatusPicking(string docentry, string status, int code, int empresa_id)
        {
            _context = new ApplicationDbContext();
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
            string connectionString = DBHelper.GetConnectionString(empresa);
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string sql = string.Format(@"update [@UPD_PCK_ETAPA] 
	                                            set U_status = '{0}'
	                                            where U_docentry = '{1}'
                                                and Code = {2}", status, docentry, code);
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        command.CommandText = sql;
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void RetornaStatusOperador(string docentry, string status, int code, int empresa_id)
        {
            _context = new ApplicationDbContext();
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
            string connectionString = DBHelper.GetConnectionString(empresa);
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string sql = string.Format(@"update [@UPD_PCK_OPERADOR] 
	                                            set U_status = '{0}'
	                                            where U_docentry = '{1}'
                                                and Code = {2}", status, docentry, code);
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        command.CommandText = sql;
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void CancelaListaPickingAberto(string docentry, int empresa_id)
        {
            _context = new ApplicationDbContext();
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
            string connectionString = DBHelper.GetConnectionString(empresa);
            int errcode;
            string errmsg;
            try
            {                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string sql = string.Format(@"select AbsEntry from PKL1 where OrderEntry = '{0}'", docentry);
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        SAPbobsCOM.Company company = Helpers.SAPHelper.GetCompany(empresa);
                        command.CommandText = sql;
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {                            
                            SAPbobsCOM.PickLists pickLists = (PickLists)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPickLists);
                            pickLists.GetByKey(int.Parse(reader["AbsEntry"].ToString()));
                            pickLists.Close();
                            company.GetLastError(out  errcode, out  errmsg);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}