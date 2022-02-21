using Picking_Web.COMObjects;
using Picking_Web.Contexto;
using Picking_Web.Helpers;
using Picking_Web.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Picking_Web.Controllers.API
{
    public class BaixaController : ApiController
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

        [System.Web.Http.HttpGet]
        public IHttpActionResult GetItensPedidoBaixa(int empresa_id, string ItemCodeLote)
        {
            try
            {
                if (!String.IsNullOrEmpty(ItemCodeLote))
                {
                    // Separando os componentes do código de barras de acordo com os caracteres               
                    string[] separacao = ItemCodeLote.Split(new char[] { ' ', '-' });     //divide por qualquer um deles
                    string _codigoItem = "";
                    string _codLote = "";
                    string _DV = "";
                    string sql = string.Empty;

                    // verifica se etiqueta está no modelo antigo ou novo
                    if (separacao.Length == 3)  //modelo novo (itemCode + CodLote + DV)
                    {
                        _codigoItem = separacao[0];
                        _codLote = separacao[1];
                        _DV = separacao[2];

                    }
                    else if (separacao.Length == 2)    //modelo antigo (CodLote + DV)
                    {
                        _codLote = separacao[0];
                        _DV = separacao[1];
                    }
                    else if (separacao.Length == 1)
                    {
                        _codLote = separacao[0];

                    }
                    else
                    {
                        return BadRequest("Código de barras lido está em formato diferente do esperado");
                    }
                    string CodLote_and_DV = $"{_codLote}-{_DV}";
                    Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

                    if (empresa == null)
                    {
                        return NotFound();
                    }

                    string connectionString = DBHelper.GetConnectionString(empresa);
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        if (conn.State == ConnectionState.Closed)
                        {
                            conn.Open();
                        }
                        switch (separacao.Length)
                        {
                            case 1:
                                sql = string.Format(
                            $@"select DistNumber,ItemCode 
                           from OBTN
                           where DistNumber = '{_codLote}' ");
                                break;
                            case 2:
                                sql = string.Format(
                           $@"select DistNumber,ItemCode 
                           from OBTN
                           where DistNumber = '{_codLote}'
                            ");
                                break;
                            case 3:
                                sql = string.Format(
                           $@"select DistNumber,ItemCode 
                           from OBTN
                           where DistNumber = '{_codLote}'
                            ");
                                break;
                        }
                        using (SqlCommand command = new SqlCommand("", conn))
                        {
                            command.CommandText = sql;
                            SqlDataReader reader = command.ExecuteReader();
                            {
                                if (reader.Read())
                                {
                                    var StringLote = reader["DistNumber"].ToString();
                           


                                }
                                else
                                {
                                    return BadRequest("A busca nao gerou resultados !");
                                }
                            }
                        }
                        if (!String.IsNullOrEmpty(empresa.DepoPadrao))
                        {
                            List<object> res = new List<object>() { };

                            using (SqlConnection conn2 = new SqlConnection(connectionString))
                            {
                                if (conn2.State == ConnectionState.Closed)
                                {
                                    conn2.Open();
                                }
                                using (SqlCommand command = new SqlCommand("", conn2))
                                {

                                    sql = string.Format(
                                        $@"
                                        select t0.ItemCode, t0.itemName, t0.DistNumber, t1.WhsCode
                                        from obtn t0
                                        inner join oitw t1 on t0.ItemCode = t1.ItemCode
                                        where t0.DistNumber = '{_codLote}'
                                        AND t1.WhsCode = '{empresa.DepoPadrao}'
                                        AND t1.OnHand > 0 ");
                                    command.CommandText = sql;
                                    SqlDataReader reader = command.ExecuteReader();
                                    int i = 0;
                                    while (reader.Read())
                                    {
                                        res.Add(new
                                        {
                                            Item = reader["ItemCode"],
                                            Descricao = reader["ItemName"],
                                            Deposito = reader["WhsCode"],
                                            numLote = reader["DistNumber"],
                                        });
                                        i++;
                                    }
                                }
                            }

                            if (res.Count > 0)
                                return Ok(new
                                {
                                    Itens = res,
                                });

                            else
                                return BadRequest("Este Item não está disponivel em estoque para o deposito padrão cadastrado");
                        }
                        else
                        {
                            return BadRequest("Este o Codigo de Item Se refere a Este Codigo de Estoque");
                        }


                        //else
                        //{
                        //    return BadRequest("Não é possível realizar a baixa para este Codigo de Item, o lote indicado não existe!");
                        //}
                    }
                }
                return BadRequest("Não é possível realizar a baixa para este Codigo de Item, o campo esta vazio ou nulo");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        [System.Web.Http.HttpGet]
        public IHttpActionResult ValidaParceiroDeNegocio(int empresa_id, string Card_Code)
        {
            try
            {

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

                if (empresa == null)
                {
                    return NotFound();
                }

                string connectionString = DBHelper.GetConnectionString(empresa);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string sql =
                        $@"select CardCode 
                           from OCRD 
                           WHERE CardCode  = '{Card_Code}'
                        ";

                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        command.CommandText = sql;
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var CardCode = reader["CardCode"].ToString();
                            if (!string.IsNullOrEmpty(CardCode))
                                return Ok();

                        }
                        return BadRequest("Este Codigo não é valido para nenhum Parceiro de Negocio cadastrado no sistema");
                    }

                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult EfetuarBaixaPN()
        {

            try
            {
                string str_empresa_id = HttpContext.Current.Request.Form.Get("empresa_id");
                int empresa_id = NumberHelper.StringToInt(str_empresa_id);

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

                if (empresa == null)
                {
                    return NotFound();
                }

                string connectionString = DBHelper.GetConnectionString(empresa);

                ParceiroDeNegocio PN = new ParceiroDeNegocio();
                PN.CardCode = HttpContext.Current.Request.Form.Get("CardCode_Pn");
                PN.Deposito = GetDepositoPN(connectionString, PN.CardCode);

                List<ItensBaixa> ListItemBaixa = new List<ItensBaixa>();
                ItensBaixa itensBaixa = new ItensBaixa();
                // 1 é a quantidade de colunas.
                var aux = HttpContext.Current.Request.Form.Count;
                var aux4 = (HttpContext.Current.Request.Form.Count - 3);
                var aux2 = ((HttpContext.Current.Request.Form.Count - 3) / 5);

                for (int i = 0; i <= ((HttpContext.Current.Request.Form.Count - 3) / 5); i++)
                {
                    string str_checked = HttpContext.Current.Request.Form.Get("dataset_itens[" + i + "][checked]");
                    if (str_checked == "true")
                    {
                        itensBaixa = new ItensBaixa();
                        itensBaixa.check = true;
                        itensBaixa.ItemCode = HttpContext.Current.Request.Form.Get("dataset_itens[" + i + "][item]");
                        itensBaixa.Descricao = HttpContext.Current.Request.Form.Get("dataset_itens[" + i + "][descricao]");
                        itensBaixa.Deposito = HttpContext.Current.Request.Form.Get("dataset_itens[" + i + "][deposito]");
                        itensBaixa.Lote = HttpContext.Current.Request.Form.Get("dataset_itens[" + i + "][numLote]");
                        ListItemBaixa.Add(itensBaixa);
                    }
                }

                if (ListItemBaixa.Count == 1)
                {

                    //using (var companyCOM = new COMCompany(empresa))
                    //{
                    //    Company oCompany = companyCOM.Company;
                    new COMCompany(empresa, true);
                    var com = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();
                    Company oCompany = com.company;
                    using (var StockTransferCOM = new COMStockTransfer(oCompany))
                    {
                        var oStockTransfer = StockTransferCOM.Transfer;
                        oStockTransfer.CardCode = PN.CardCode;
                        var Linha = 0;
                        foreach (var item in ListItemBaixa)
                        {
                            oStockTransfer.Lines.ItemCode = item.ItemCode;
                            oStockTransfer.Lines.FromWarehouseCode = item.Deposito;
                            oStockTransfer.Lines.WarehouseCode = PN.Deposito;
                            oStockTransfer.Lines.Quantity = 1;

                            oStockTransfer.Lines.SetCurrentLine(Linha);
                            oStockTransfer.Lines.BatchNumbers.BatchNumber = item.Lote;
                            oStockTransfer.Lines.BatchNumbers.Quantity = 1;
                            oStockTransfer.Lines.BatchNumbers.BaseLineNumber = Linha;
                            oStockTransfer.Lines.BatchNumbers.Add();

                            oStockTransfer.Lines.Add();

                            Linha++;
                        }
                        int err = oStockTransfer.Add();
                        if (err != 0)
                        {
                            oCompany.GetLastError(out err, out string msg);
                            return BadRequest(msg);
                        }
                        else
                        {
                            return Ok();

                        }

                    }

                }
                else if (ListItemBaixa.Count < 1)
                {
                    return BadRequest("Nenhum Item foi selecionado para Baixa no Deposito do PN");
                }
                else
                {
                    return BadRequest("Gentileza marcar apenas 1 item para  Baixa no Deposito do PN");
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        public string GetDepositoPN(string connectionString, string CardCode)
        {
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                var DeositoPn = "";
                if (conn2.State == ConnectionState.Closed)
                {
                    conn2.Open();
                }
                using (SqlCommand command = new SqlCommand("", conn2))
                {
                    string sql = string.Format(@"select U_UPD_TdWC from OCRD where CardCode = '{0}'", CardCode);

                    command.CommandText = sql;
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        DeositoPn = reader["U_UPD_TdWC"].ToString();

                    }
                }
                return DeositoPn;
            }
        }
    }
}