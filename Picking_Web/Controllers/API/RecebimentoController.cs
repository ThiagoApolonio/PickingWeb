using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Picking_Web.COMObjects;
using Picking_Web.Contexto;
using Picking_Web.Helpers;
using Picking_Web.Models;
using Picking_Web.PrinterLayout;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Configuration;

namespace Picking_Web.Controllers.API
{
    public class RecebimentoController : ApiController
    {
        #region :: Propriedades e Construtor

        private ApplicationDbContext _context;
        private List<Dictionary<string, string>> dataset_impressao_global;
        private List<Dictionary<string, string>> dataset_impressao_ambiente;
        private List<Dictionary<string, string>> dataset_impressao_geladeira;

        public RecebimentoController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        #endregion


        #region :: Funções


        [System.Web.Http.HttpGet]
        public IHttpActionResult GetNotaFiscal(int empresa_id, string numnota)
        {
            try
            {
                if (String.IsNullOrEmpty(numnota))
                {
                    return Ok(new { });
                }

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

                if (empresa == null)
                {
                    return NotFound();
                }

                string connectionString = Helpers.DBHelper.GetConnectionString(empresa);
                List<object> itens_nota = new List<object>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    //área de teste-------------------------------------------
                    var DocType = (int)SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes;
                    var ManagedBy = (int)SAPbobsCOM.ServiceTypes.BatchNumberDetailsService;

                    var DocTypes = (int)SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes;
                    var ManagedBys = (int)SAPbobsCOM.ServiceTypes.SerialNumberDetailsService;

                    string sql =
                    $@"SELECT 
	                    tb0.ItemCode
	                    , tb0.ItemName
	                    , tb1.Quantity as Quantidade
	                    , tb3.unitMsr as UnidadeMedida
	                    , tb2.DistNumber as numLote
	                    , tb2.MnfSerial as CodBarras
	                    , isnull(tb2.expdate,'20991231') as DataVenc
	                    , Cast(Isnull(Tb4.U_UPD_PCK_ARMAZ,'A') as varchar) as Ambiente
	                    , tb3.WhsCode as Deposito
                        , ISNULL(tb5.FirmName,'--') as Fornecedor
	                    , ISNULL(tb2.MnfSerial, '--') as loteFabricante
                    FROM OITL tb0
                    INNER JOIN ITL1 tb1 ON (tb1.LogEntry = tb0.LogEntry)
                    INNER JOIN OBTN tb2 ON (tb2.AbsEntry = tb1.MdAbsEntry)
                    INNER JOIN PDN1 tb3 ON (tb3.LineNum = tb0.DocLine AND tb3.DocEntry = tb0.DocEntry )
                    INNER JOIN OITM tb4 ON (tb4.ItemCode = tb0.ItemCode)
                    LEFT JOIN OMRC tb5 ON (tb5.FirmCode = tb4.FirmCode )
                    WHERE 1 = 1
	                    AND tb0.DocEntry = {numnota}
                        AND tb0.DocType = {(int)SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes}
                        AND tb0.ManagedBy = {(int)SAPbobsCOM.ServiceTypes.BatchNumberDetailsService}
                        
                    UNION

                    
                    SELECT 
	                    tb0.ItemCode
	                    , tb0.ItemName
	                    , tb1.Quantity as Quantidade
	                    , tb3.unitMsr as UnidadeMedida
	                    , tb2.DistNumber as numLote
	                    , tb2.MnfSerial as CodBarras
	                    , isnull(tb2.expdate,'20991231') as DataVenc
	                    , Cast(Isnull(Tb4.U_UPD_PCK_ARMAZ,'A') as varchar) as Ambiente
	                    , tb3.WhsCode as Deposito
                        , ISNULL(tb5.FirmName,'--') as Fornecedor
	                    , ISNULL(tb2.MnfSerial, '--') as loteFabricante
                    FROM OITL tb0
                    INNER JOIN ITL1 tb1 ON (tb1.LogEntry = tb0.LogEntry)
                    INNER JOIN OSRN tb2 ON (tb2.AbsEntry = tb1.MdAbsEntry)
                    INNER JOIN PDN1 tb3 ON (tb3.LineNum = tb0.DocLine AND tb3.DocEntry = tb0.DocEntry )
                    INNER JOIN OITM tb4 ON (tb4.ItemCode = tb0.ItemCode)
                    LEFT JOIN OMRC tb5 ON (tb5.FirmCode = tb4.FirmCode )
                    WHERE 1 = 1
	                    AND tb0.DocEntry = {numnota}
                        AND tb0.DocType = {(int)SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes}
                        AND tb0.ManagedBy = {(int)SAPbobsCOM.ServiceTypes.SerialNumberDetailsService}


                    ";
                    //Log
                    new GravarLog().Escrever("\n + DocEntry: " + numnota + "\n" +
                        "DocType: " + (int)SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes + "\n" +
                        "ManagedBy1: " + (int)SAPbobsCOM.ServiceTypes.BatchNumberDetailsService + "\n" +
                        "ManagedBy2: " + (int)SAPbobsCOM.ServiceTypes.SerialNumberDetailsService + "\n"
                        );

                    string sq = sql;
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        command.CommandText = sql;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                itens_nota.Add(new
                                {
                                    ItemCode = reader["ItemCode"].ToString(),
                                    ItemName = reader["ItemName"].ToString(),
                                    Quantidade = NumberHelper.GetFromDBToDouble(reader["Quantidade"]),
                                    UnidadeMedida = reader["UnidadeMedida"].ToString(),
                                    numLote = reader["numLote"].ToString(),
                                    CodBarras = reader["CodBarras"].ToString(),
                                    DataVenc = reader["DataVenc"].ToString().Substring(0, 10),
                                    Ambiente = reader["Ambiente"].ToString(),
                                    Deposito = reader["Deposito"].ToString(),
                                    LoteFabricante = reader["LoteFabricante"].ToString(),
                                    Fornecedor = reader["Fornecedor"].ToString(),
                                });
                            }
                        }
                    }
                }


                return Ok(new
                {
                    Itens = itens_nota,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //
        // GET: /Recebimento/GetLotes
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetLotes(int empresa_id, string itemcode)
        {
            try
            {
                if (string.IsNullOrEmpty(itemcode))
                {
                    return Ok(new { });
                }

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

                if (empresa == null)
                {
                    return NotFound();
                }

                List<string> lotes = new List<string>() { };
                string connectionString = Helpers.DBHelper.GetConnectionString(empresa);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string sql =
                    $@"select T0.ItemCode, Sum(T1.quantity), T2.DistNumber from oitl T0
                        inner join itl1 T1 on T0.LogEntry = T1.LogEntry
                        left Join OBTN T2 on T1.SysNumber = T2.SysNumber and T2.AbsEntry = T1.MdAbsEntry
                        where t0.ItemCode = '{itemcode}'
                        group by t2.DistNumber, T0.ItemCode
                        Having sum(T1.quantity)>0
                        UNION
                        select T0.ItemCode, Sum(T1.quantity), T2.DistNumber from oitl T0
                        inner join itl1 T1 on T0.LogEntry = T1.LogEntry
                        left Join OSRN T2 on T1.SysNumber = T2.SysNumber and T2.AbsEntry = T1.MdAbsEntry
                        where t0.ItemCode = '{itemcode}'
                        group by t2.DistNumber, T0.ItemCode
                        Having sum(T1.quantity)>0
                        ORDER BY T2.DistNumber asc";
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        command.CommandText = sql;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lotes.Add(reader["DistNumber"].ToString());
                            }
                        }
                    }
                }

                if (lotes.Count > 0)
                return Ok(new
                {
                    lotes = lotes,
                });
                else
                    return BadRequest("Não foi encontrado nenhum lote para o item: " + itemcode);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult GetLotesInfo(int empresa_id, string numlote, string ItemCode)
        {
            try
            {
                if (string.IsNullOrEmpty(numlote))
                {
                    return Ok(new { });
                }

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

                if (empresa == null)
                {
                    return NotFound();
                }

                List<object> lote_info = new List<object>();
                string connectionString = Helpers.DBHelper.GetConnectionString(empresa);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string sql =
                    $@"SELECT 
	                        tb1.ItemCode
	                        , tb1.ItemName
	                        , tb2.BuyUnitMsr as UnidadeMedida
	                        , tb1.lotNumber as CodBarras
	                        , ISNULL(CONVERT(VARCHAR(10),tb1.ExpDate,103),'--') as DataVenc
	                        , Cast(tb2.U_UPD_PCK_ARMAZ as varchar) as Ambiente
	                        , tb2.DfltWH as Deposito
	                        , ISNULL(tb3.FirmName,'--') as Fornecedor
	                        , ISNULL(tb1.MnfSerial, '--') as LoteFabricante
                        FROM OBTN tb1
                        INNER JOIN OITM tb2 ON (tb2.ItemCode = tb1.ItemCode)
                        LEFT JOIN OMRC tb3 ON (tb3.FirmCode = tb2.FirmCode )
                        WHERE tb1.DistNumber = '{numlote}' and tb1.ItemCode = '{ItemCode}'
                        
                        UNION

                        SELECT 
	                        tb1.ItemCode
	                        , tb1.ItemName
	                        , tb2.BuyUnitMsr as UnidadeMedida
	                        , tb1.lotNumber as CodBarras
	                        , ISNULL(CONVERT(VARCHAR(10),tb1.ExpDate,103),'--') as DataVenc
	                        , Cast(tb2.U_UPD_PCK_ARMAZ as varchar) as Ambiente
	                        , tb2.DfltWH as Deposito
	                        , ISNULL(tb3.FirmName,'--') as Fornecedor
	                        , ISNULL(tb1.MnfSerial, '--') as LoteFabricante
                        FROM OSRN tb1
                        INNER JOIN OITM tb2 ON (tb2.ItemCode = tb1.ItemCode)
                        LEFT JOIN OMRC tb3 ON (tb3.FirmCode = tb2.FirmCode )
                        WHERE tb1.DistNumber = '{numlote}' and tb1.ItemCode = '{ItemCode}'";
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        command.CommandText = sql;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lote_info.Add(new
                                {
                                    ItemCode = reader["ItemCode"].ToString(),
                                    ItemName = reader["ItemName"].ToString(),
                                    UnidadeMedida = reader["UnidadeMedida"].ToString(),
                                    CodBarras = reader["CodBarras"].ToString(),
                                    DataVenc = reader["DataVenc"].ToString(),
                                    Ambiente = reader["Ambiente"].ToString(),
                                    Deposito = reader["Deposito"].ToString(),
                                    LoteFabricante = reader["LoteFabricante"].ToString(),
                                    Fornecedor = reader["Fornecedor"].ToString(),
                                });
                            }
                        }
                    }
                }

                return Ok(lote_info);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetLotesInfoReimpressao(int empresa_id, string numlote, string itemCode)
        {
            try
            {
                if (string.IsNullOrEmpty(numlote))
                {
                    return Ok(new { });
                }

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

                if (empresa == null)
                {
                    return NotFound();
                }

                List<object> lote_info = new List<object>();
                string connectionString = Helpers.DBHelper.GetConnectionString(empresa);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string sql =
                    $@" select T0.U_ItemCode as ItemCode,
                        T1.ItemName as ItemName,
                        T0.U_NumLote as numlote,
                        T0.U_CodigoProduto as CodBarras,
                        T1.U_UPD_PCK_ARMAZ as Ambiente
                        from [@UPD_LOTE] T0
                        inner join oitm T1 on t0.U_ItemCode = T1.ItemCode 
                        where T0.U_NumLote = '{numlote}' and t0.U_ItemCode = '{itemCode}'";
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        command.CommandText = sql;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lote_info.Add(new
                                {
                                    ItemCode = reader["ItemCode"].ToString(),
                                    ItemName = reader["ItemName"].ToString(),
                                    numlote = reader["numlote"].ToString(),
                                    CodBarras = reader["CodBarras"].ToString(),
                                    Ambiente = reader["Ambiente"].ToString(),
                                });
                            }
                        }
                    }
                }
                if (lote_info.Count > 0)
                    return Ok(new
                    {
                        Itens = lote_info,
                    });
                else
                    return BadRequest("Este lote ainda não contem nenhuma etiqueta impressa. \nGentileza retornar a pagina principal e fazer a primeira impressão.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult GetCodBarrasInfo(int empresa_id, string codbarras)
        {
            try
            {
                if (!string.IsNullOrEmpty(codbarras))
                {

                    Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

                    if (empresa == null)
                    {
                        return NotFound();
                    }

                    List<object> lote_info = new List<object>();
                    string connectionString = Helpers.DBHelper.GetConnectionString(empresa);

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        if (conn.State == ConnectionState.Closed)
                        {
                            conn.Open();
                        }

                        string sql =
                        $@"SELECT
	                    tb1.ItemCode
	                    , tb1.ItemName
	                    , tb2.BuyUnitMsr as UnidadeMedida
	                    , tb1.MnfSerial as CodBarras
	                    , tb1.DistNumber as NumLote
	                    , ISNULL(CONVERT(VARCHAR(10),tb1.ExpDate,103),'--') as DataVenc
	                    , Cast(tb2.U_UPD_PCK_ARMAZ as varchar) as Ambiente
	                    , tb2.DfltWH as Deposito
                    FROM OBTN tb1
                    INNER JOIN OITM tb2 ON (tb2.ItemCode = tb1.ItemCode)
                    WHERE tb1.LotNumber = '{codbarras}'";


                        using (SqlCommand command = new SqlCommand("", conn))
                        {
                            command.CommandText = sql;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    lote_info.Add(new
                                    {
                                        ItemCode = reader["ItemCode"].ToString(),
                                        ItemName = reader["ItemName"].ToString(),
                                        UnidadeMedida = reader["UnidadeMedida"].ToString(),
                                        NumLote = reader["NumLote"].ToString(),
                                        CodBarras = reader["CodBarras"].ToString(),
                                        DataVenc = reader["DataVenc"].ToString(),
                                        Ambiente = reader["Ambiente"].ToString(),
                                        Deposito = reader["Deposito"].ToString(),
                                    });
                                    return Ok(lote_info);
                                }
                                return BadRequest("Valor de codigo de barras incorreto");
                            }
                        }
                    }

                }
                return BadRequest("Campo Codigo De Barras Null ou Vazio");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //[System.Web.Http.HttpPost]
        //public IHttpActionResult Imprimir()
        //{
        //    dataset_impressao_global = new List<Dictionary<string, string>>() { };
        //    dataset_impressao_ambiente = new List<Dictionary<string, string>>() { };
        //    dataset_impressao_geladeira = new List<Dictionary<string, string>>() { };
        //    List<ItensRecebimento> lstdItensRecebGlobal = new List<ItensRecebimento>();
        //    List<ItensRecebimento> lstdItensRecebAmbiente = new List<ItensRecebimento>();
        //    List<ItensRecebimento> lstdItensRecebGeladeira = new List<ItensRecebimento>();

        //    NameValueCollection httpContextForm = HttpContext.Current.Request.Form;

        //    string nome_impressora_ambiente = httpContextForm.Get("nome_impressora_ambiente");
        //    string nome_impressora_geladeira = httpContextForm.Get("nome_impressora_geladeira");
        //    string str_empresa_id = httpContextForm.Get("empresa_id");
        //    int empresa_id = NumberHelper.StringToInt(str_empresa_id);
        //    Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
        //    string str_erro = "";

        //    if (empresa == null)
        //    {
        //        return NotFound();
        //    }
        //    try
        //    {
        //        for (int i = 0; i < ((httpContextForm.Count - 3) / 13); i++)
        //        {
        //            string str_checked = httpContextForm.Get("dataset_itens[" + i + "][checked]");

        //            if (str_checked == "false")
        //            {
        //                continue;
        //            }

        //            string itemcode = httpContextForm.Get("dataset_itens[" + i + "][itemCode]");
        //            string itemname = httpContextForm.Get("dataset_itens[" + i + "][itemName]");
        //            string unid_med = httpContextForm.Get("dataset_itens[" + i + "][unidadeMedida]");
        //            string data_venc = httpContextForm.Get("dataset_itens[" + i + "][dataVenc]");
        //            string num_lote = httpContextForm.Get("dataset_itens[" + i + "][numLote]");
        //            string ambiente = httpContextForm.Get("dataset_itens[" + i + "][ambiente]");
        //            string str_quantidade = httpContextForm.Get("dataset_itens[" + i + "][quantidade]");
        //            string lote_fabricante = httpContextForm.Get("dataset_itens[" + i + "][loteFabricante]");
        //            string fornecedor = httpContextForm.Get("dataset_itens[" + i + "][fornecedor]");

        //            double quantidade = NumberHelper.StringToDouble(str_quantidade);

        //            for (int j = 0; j < quantidade; j++)
        //            {
        //                Dictionary<string, string> val = new Dictionary<string, string>() {
        //                    { "itemCode", itemcode},
        //                    { "itemName", itemname},
        //                    { "unidadeMedida", unid_med},
        //                    { "dataVenc", data_venc},
        //                    { "numLote", num_lote},
        //                    { "ambiente", ambiente},
        //                    { "loteFabricante", lote_fabricante},
        //                    { "fornecedor", fornecedor},
        //                };

        //                if (ambiente.ToUpper() == "A")
        //                {
        //                    dataset_impressao_ambiente.Add(val);
        //                }
        //                else if (ambiente.ToUpper() == "G")
        //                {
        //                    dataset_impressao_geladeira.Add(val);
        //                }
        //                else
        //                {
        //                    str_erro = $"O item {itemcode} - {itemname} não possui uma configuração de ambiente correta. Geração de etiquetas cancelada.";
        //                    break;
        //                }

        //                dataset_impressao_global.Add(val);
        //            }
        //        }

        //        if (String.IsNullOrEmpty(str_erro))
        //        {
        //            if (dataset_impressao_ambiente.Count > 0)
        //            {
        //                RealizarImpressao(dataset_impressao_ambiente, nome_impressora_ambiente, empresa);
        //            }
        //            if (dataset_impressao_geladeira.Count > 0)
        //            {
        //                RealizarImpressao(dataset_impressao_geladeira, nome_impressora_geladeira, empresa);
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest(str_erro);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    return Ok();
        //}


        // ALTERADO PARA CONTROLE DE LOTE 


        //[System.Web.Http.HttpPost]
        //public IHttpActionResult Imprimir()
        //{
        //    dataset_impressao_global = new List<Dictionary<string, string>>() { };
        //    dataset_impressao_ambiente = new List<Dictionary<string, string>>() { };
        //    dataset_impressao_geladeira = new List<Dictionary<string, string>>() { };
        //    List<ItensRecebimento> lstdItensRecebGlobal = new List<ItensRecebimento>();
        //    List<ItensRecebimento> lstdItensRecebAmbiente = new List<ItensRecebimento>();
        //    List<ItensRecebimento> lstdItensRecebGeladeira = new List<ItensRecebimento>();

        //    NameValueCollection httpContextForm = HttpContext.Current.Request.Form;

        //    string nome_impressora_ambiente = httpContextForm.Get("nome_impressora_ambiente");
        //    string nome_impressora_geladeira = httpContextForm.Get("nome_impressora_geladeira");
        //    string str_empresa_id = httpContextForm.Get("empresa_id");
        //    int empresa_id = NumberHelper.StringToInt(str_empresa_id);
        //    Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
        //    string str_erro = "";

        //    if (empresa == null)
        //    {
        //        return NotFound();
        //    }
        //    try
        //    {
        //        for (int i = 0; i < ((httpContextForm.Count - 3) / 13); i++)
        //        {
        //            string str_checked = httpContextForm.Get("dataset_itens[" + i + "][checked]");

        //            if (str_checked == "false")
        //            {
        //                continue;
        //            }

        //            string itemcode = httpContextForm.Get("dataset_itens[" + i + "][itemCode]");
        //            string itemname = httpContextForm.Get("dataset_itens[" + i + "][itemName]");
        //            string unid_med = httpContextForm.Get("dataset_itens[" + i + "][unidadeMedida]");
        //            string data_venc = httpContextForm.Get("dataset_itens[" + i + "][dataVenc]");
        //            string num_lote = httpContextForm.Get("dataset_itens[" + i + "][numLote]");
        //            string ambiente = httpContextForm.Get("dataset_itens[" + i + "][ambiente]");
        //            string str_quantidade = httpContextForm.Get("dataset_itens[" + i + "][quantidade]");
        //            string lote_fabricante = httpContextForm.Get("dataset_itens[" + i + "][loteFabricante]");
        //            string fornecedor = httpContextForm.Get("dataset_itens[" + i + "][fornecedor]");

        //            double quantidade = NumberHelper.StringToDouble(str_quantidade);

        //            ItensRecebimento itensReceb = new ItensRecebimento();
        //            itensReceb.ItemCode = itemcode;
        //            itensReceb.ItemName = itemname;
        //            itensReceb.UnidMed = unid_med;
        //            itensReceb.DataVenc = data_venc;
        //            itensReceb.NumLote = num_lote;
        //            itensReceb.Ambiente = ambiente;
        //            itensReceb.LoteFab = lote_fabricante;
        //            itensReceb.Fornecedor = fornecedor;

        //            if (ambiente.ToUpper() == "A")
        //            {
        //                lstdItensRecebAmbiente.Add(itensReceb);
        //            }
        //            else if (ambiente.ToUpper() == "G")
        //            {
        //                lstdItensRecebGeladeira.Add(itensReceb);
        //            }
        //            else
        //            {
        //                str_erro = $"O item {itemcode} - {itemname} não possui uma configuração de ambiente correta. Geração de etiquetas cancelada.";
        //                break;
        //            }

        //            lstdItensRecebGlobal.Add(itensReceb);
        //        }


        //        if (String.IsNullOrEmpty(str_erro))
        //        {
        //            if (lstdItensRecebAmbiente.Count > 0)
        //            {
        //                RealizarImpressao(lstdItensRecebAmbiente, nome_impressora_ambiente, empresa);
        //            }
        //            if (lstdItensRecebGeladeira.Count > 0)
        //            {
        //                RealizarImpressao(lstdItensRecebGeladeira, nome_impressora_geladeira, empresa);
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest(str_erro);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    return Ok();
        //}
        [System.Web.Http.HttpPost]
        public IHttpActionResult Imprimir()
        {
            Startup._logFile = new GravarLog().Escrever("Inicio da chamada Imprimir");
            dataset_impressao_global = new List<Dictionary<string, string>>() { };
            dataset_impressao_ambiente = new List<Dictionary<string, string>>() { };
            dataset_impressao_geladeira = new List<Dictionary<string, string>>() { };
            List<ItensRecebimento> lstdItensRecebGlobal = new List<ItensRecebimento>();
            List<ItensRecebimento> lstdItensRecebAmbiente = new List<ItensRecebimento>();
            List<ItensRecebimento> lstdItensRecebGeladeira = new List<ItensRecebimento>();

            NameValueCollection httpContextForm = HttpContext.Current.Request.Form;

            string nome_impressora_ambiente = httpContextForm.Get("nome_impressora_ambiente");
            string nome_impressora_geladeira = httpContextForm.Get("nome_impressora_geladeira");
            string str_empresa_id = httpContextForm.Get("empresa_id");
            int empresa_id = NumberHelper.StringToInt(str_empresa_id);
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
            string connectionString = DBHelper.GetConnectionString(empresa);
            string str_erro = "";

            if (empresa == null)
            {
                return NotFound();
            }
            try
            {
                var aux = (httpContextForm.Count - 3) / 13;
                for (int i = 0; i < ((httpContextForm.Count - 3) / 13); i++)
                {
                    var aux2 = (httpContextForm.Count - 3) / 13;
                    string str_checked = httpContextForm.Get("dataset_itens[" + i + "][checked]");

                    if (str_checked == "false")
                    {
                        continue;
                    }

                    string itemcode = httpContextForm.Get("dataset_itens[" + i + "][itemCode]");
                    string itemname = httpContextForm.Get("dataset_itens[" + i + "][itemName]");
                    string unid_med = httpContextForm.Get("dataset_itens[" + i + "][unidadeMedida]");
                    string data_venc = httpContextForm.Get("dataset_itens[" + i + "][dataVenc]");
                    string num_lote = httpContextForm.Get("dataset_itens[" + i + "][numLote]");
                    string ambiente = httpContextForm.Get("dataset_itens[" + i + "][ambiente]"); //original
                  /*  string ambiente = "a"*/;//teste
                    string str_quantidade = httpContextForm.Get("dataset_itens[" + i + "][quantidade]");
                    string lote_fabricante = httpContextForm.Get("dataset_itens[" + i + "][loteFabricante]");
                    string fornecedor = httpContextForm.Get("dataset_itens[" + i + "][fornecedor]");

                    double quantidade = NumberHelper.StringToDouble(str_quantidade);


                    for (int j = 0; j < quantidade; j++)
                    {
                        ItensRecebimento itensReceb = new ItensRecebimento();
                        itensReceb.ItemCode = itemcode;
                        itensReceb.ItemName = itemname;
                        itensReceb.UnidMed = unid_med;
                        itensReceb.DataVenc = data_venc;
                        itensReceb.NumLote = num_lote;
                        itensReceb.Ambiente = ambiente;
                        itensReceb.LoteFab = lote_fabricante;
                        itensReceb.Fornecedor = fornecedor;

                        if (ambiente.ToUpper() == "A")
                        {
                            lstdItensRecebAmbiente.Add(itensReceb);
                        }
                        else if (ambiente.ToUpper() == "G")
                        {
                            lstdItensRecebGeladeira.Add(itensReceb);
                        }
                        else
                        {
                            str_erro = $"O item {itemcode} - {itemname} não possui uma configuração de ambiente correta. Geração de etiquetas cancelada.";
                            break;
                        }

                        lstdItensRecebGlobal.Add(itensReceb);
                    }
                }


                if (String.IsNullOrEmpty(str_erro))
                {
                    if (lstdItensRecebAmbiente.Count > 0)
                    {
                        RealizarImpressao(lstdItensRecebAmbiente, nome_impressora_ambiente, empresa);


                    }
                    if (lstdItensRecebGeladeira.Count > 0)
                    {
                        RealizarImpressao(lstdItensRecebGeladeira, nome_impressora_geladeira, empresa);

                    }
                }
                else
                {
                    return BadRequest(str_erro);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult ReImprimir()
        {
            Startup._logFile = new GravarLog().Escrever("Inicio da chamada Imprimir");
            dataset_impressao_global = new List<Dictionary<string, string>>() { };
            dataset_impressao_ambiente = new List<Dictionary<string, string>>() { };
            dataset_impressao_geladeira = new List<Dictionary<string, string>>() { };
            List<ItensRecebimento> lstdItensRecebGlobal = new List<ItensRecebimento>();
            List<ItensRecebimento> lstdItensRecebAmbiente = new List<ItensRecebimento>();
            List<ItensRecebimento> lstdItensRecebGeladeira = new List<ItensRecebimento>();

            NameValueCollection httpContextForm = HttpContext.Current.Request.Form;

            string nome_impressora_ambiente = httpContextForm.Get("nome_impressora_ambiente");
            string nome_impressora_geladeira = httpContextForm.Get("nome_impressora_geladeira");
            string str_empresa_id = httpContextForm.Get("empresa_id");
            int empresa_id = NumberHelper.StringToInt(str_empresa_id);
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
            string connectionString = DBHelper.GetConnectionString(empresa);
            string str_erro = "";

            if (empresa == null)
            {
                return NotFound();
            }
            try
            {
                var aux = (httpContextForm.Count - 3) / 6;
                for (int i = 0; i < ((httpContextForm.Count - 3) / 6); i++)
                {
                    var aux2 = (httpContextForm.Count - 3) / 6;
                    string str_checked = httpContextForm.Get("dataset_itens[" + i + "][checked]");

                    if (str_checked == "false")
                    {
                        continue;
                    }

                    string itemcode = httpContextForm.Get("dataset_itens[" + i + "][itemCode]");
                    string itemname = httpContextForm.Get("dataset_itens[" + i + "][itemName]");
                    string num_lote = httpContextForm.Get("dataset_itens[" + i + "][numLote]");
                    string ambiente = httpContextForm.Get("dataset_itens[" + i + "][ambiente]");
                    string CodDeBarras = httpContextForm.Get("dataset_itens[" + i + "][codBarras]");


                    ItensRecebimento itensReceb = new ItensRecebimento();
                    itensReceb.ItemCode = itemcode;
                    itensReceb.ItemName = itemname;
                    itensReceb.NumLote = num_lote;
                    itensReceb.CodigoItem = CodDeBarras;
                    itensReceb.Ambiente = ambiente;

                    if (ambiente.ToUpper() == "A")
                    {
                        lstdItensRecebAmbiente.Add(itensReceb);
                    }
                    else if (ambiente.ToUpper() == "G")
                    {
                        lstdItensRecebGeladeira.Add(itensReceb);
                    }
                    else
                    {
                        str_erro = $"O item {itemcode} - {itemname} não possui uma configuração de ambiente correta. Geração de etiquetas cancelada.";
                        break;
                    }

                    lstdItensRecebGlobal.Add(itensReceb);
                }


                if (String.IsNullOrEmpty(str_erro))
                {
                    if (lstdItensRecebAmbiente.Count > 0)
                    {
                        RealizarReimpressao(lstdItensRecebAmbiente, nome_impressora_ambiente, empresa);


                    }
                    if (lstdItensRecebGeladeira.Count > 0)
                    {
                        RealizarReimpressao(lstdItensRecebGeladeira, nome_impressora_geladeira, empresa);

                    }
                }
                else
                {
                    return BadRequest(str_erro);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
        #endregion

        #region :: Auxiliares


        ////QR CODE SEM CONTROLE DE LOTE (AMANTER)
        //private void RealizarImpressao(List<ItensRecebimento> lstdItens, string nome_impressora, Empresa empresa)
        //{
        //    Startup._logFile = new GravarLog(true).Escrever("Inicio realização impressão");
        //    string connectionString = DBHelper.GetConnectionString(empresa);
        //    Startup._logFile = new GravarLog(true).Escrever("ConnectionString: " + connectionString);
        //    new COMCompany(empresa, true);
        //    Startup._logFile = new GravarLog(true).Escrever("empresa: " + empresa);
        //    var com = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();
        //    if (com != null)
        //    {
        //        Startup._logFile = new GravarLog(true).Escrever("Empresa : " + com.company.CompanyName);
        //    }
        //    else
        //    {
        //        Startup._logFile = new GravarLog(true).Escrever("Objeto de empresa não localizado");
        //        throw new Exception("Objeto de empresa não localizado");
        //    }
        //    Company oCompany = com.company;
        //    Startup._logFile = new GravarLog(true).Escrever("company " + oCompany);
        //    string ReportPath = GlobalHelper.RelatoriosPath + "/Etiqueta.rpt";
        //    Startup._logFile = new GravarLog(true).Escrever("Path: " + ReportPath);
        //    if (System.IO.File.Exists(ReportPath))
        //    {
        //        try
        //        {
        //            foreach (var item in lstdItens)
        //            {

        //                var qr = new QrCode();
        //                if (!Directory.Exists(qr.Directory))
        //                {
        //                    Startup._logFile = new GravarLog(true).Escrever("Diretorio não existe");
        //                    Directory.CreateDirectory(qr.Directory);
        //                    Startup._logFile = new GravarLog(true).Escrever("Diretorio Criado");
        //                }
        //                string qrcode = qr.Directory + "/Qr_" + item.NumLote + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        //                var img = new QrCode().GenerateQrCode(qr.Width, qr.Height, item.NumLote, ZXing.BarcodeFormat.QR_CODE);
        //                img.Save(qrcode);

        //                using (var _crystal = new ReportDocument())
        //                {
        //                    _crystal.Load(ReportPath);
        //                    System.Drawing.Printing.PrintDocument pDoc = new System.Drawing.Printing.PrintDocument();
        //                    PrintLayoutSettings PrintLayout = new PrintLayoutSettings();
        //                    System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();

        //                    _crystal.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
        //                    printerSettings.PrinterName = nome_impressora;
        //                    System.Drawing.Printing.PageSettings pSettings = new System.Drawing.Printing.PageSettings(printerSettings);
        //                    _crystal.PrintOptions.DissociatePageSizeAndPrinterPaperSize = true;
        //                    _crystal.PrintOptions.PrinterDuplex = PrinterDuplex.Simplex;

        //                    _crystal.SetDatabaseLogon(empresa.UsuarioBanco, empresa.SenhaBanco, empresa.InstanciaBanco, empresa.NomeBanco);
        //                    _crystal.DataSourceConnections[0].SetConnection(empresa.InstanciaBanco, empresa.NomeBanco, empresa.UsuarioBanco, empresa.SenhaBanco);
        //                    _crystal.DataSourceConnections[0].SetLogon(empresa.UsuarioBanco, empresa.SenhaBanco);
        //                    _crystal.SetParameterValue("Cod", item.ItemCode);
        //                    _crystal.SetParameterValue("Descricao", item.ItemName);
        //                    _crystal.SetParameterValue("NumLote", item.NumLote);
        //                    _crystal.SetParameterValue("QrCode", qrcode);
        //                    //_crystal.SetParameterValue("Densidade", item);
        //                    //_crystal.SetParameterValue("Peso", item.Pes);
        //                    //string pdf = string.Format(@"{0}\{1}.pdf", PrintDirectory.Directory, "TAG_" + DateTime.Now.ToString("yyyyMMddHHmmsss"));

        //                    //_crystal.ExportToDisk(ExportFormatType.PortableDocFormat, pdf);

        //                    _crystal.PrintToPrinter(printerSettings, pSettings, false, PrintLayout);
        //                    _crystal.Close();
        //                    File.Delete(qrcode);

        //                }
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            //crReport = null;
        //            GC.Collect();
        //        }
        //    }
        //}


        //ADAPTADO PARA CCONTROLE DE LOTE + CODIGO DE BARRAS (UNIONLAB)
        private void RealizarImpressao(List<ItensRecebimento> lstdItens, string nome_impressora, Empresa empresa)
        {
            Startup._logFile = new GravarLog(true).Escrever("Inicio realização impressão");
            string connectionString = DBHelper.GetConnectionString(empresa);
            Startup._logFile = new GravarLog(true).Escrever("ConnectionString: " + connectionString);
            new COMCompany(empresa, true);
            Startup._logFile = new GravarLog(true).Escrever("empresa: " + empresa);
            var com = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();
            if (com != null)
            {
                Startup._logFile = new GravarLog(true).Escrever("Empresa : " + com.company.CompanyName);
            }
            else
            {
                Startup._logFile = new GravarLog(true).Escrever("Objeto de empresa não localizado");
                throw new Exception("Objeto de empresa não localizado");
            }
            Company oCompany = com.company;
            Startup._logFile = new GravarLog(true).Escrever("company " + oCompany);

            string ReportPath;
            //verificando tipo de leitura
            String leitura = ConfigurationManager.AppSettings["LEITURA"].ToString();
            if (leitura == "QRCODE")
            {
                ReportPath = GlobalHelper.RelatoriosPath + "/EtiquetaQrCode.rpt";
            }
            else
            {
                ReportPath = GlobalHelper.RelatoriosPath + "/Etiqueta.rpt";
            }

                Startup._logFile = new GravarLog(true).Escrever("Path: " + ReportPath);
            var imglote = "";
            if (System.IO.File.Exists(ReportPath)) 
            {
                try
                {
                    foreach (var item in lstdItens)
                    {
                        if (oCompany.InTransaction)
                            oCompany.StartTransaction();

                        if (empresa.ContadorLote)
                        {
                            item.Digito = new PickingController().CalculaDigitoLote(connectionString, item.NumLote,item.ItemCode);
                            item.CodigoItem = string.Join("-", item.NumLote, item.Digito.ToString());
                            new DataBaseFunctions(oCompany).AdcControleLote(item);
                        }

                        using (var _crystal = new ReportDocument())
                        {
                            _crystal.Load(ReportPath);
                            System.Drawing.Printing.PrintDocument pDoc = new System.Drawing.Printing.PrintDocument();
                            PrintLayoutSettings PrintLayout = new PrintLayoutSettings();
                            System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();

                            _crystal.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                            printerSettings.PrinterName = nome_impressora;
                            System.Drawing.Printing.PageSettings pSettings = new System.Drawing.Printing.PageSettings(printerSettings);
                            _crystal.PrintOptions.DissociatePageSizeAndPrinterPaperSize = true;
                            _crystal.PrintOptions.PrinterDuplex = PrinterDuplex.Simplex;

                            _crystal.SetDatabaseLogon(empresa.UsuarioBanco, empresa.SenhaBanco, empresa.InstanciaBanco, empresa.NomeBanco);
                            _crystal.DataSourceConnections[0].SetConnection(empresa.InstanciaBanco, empresa.NomeBanco, empresa.UsuarioBanco, empresa.SenhaBanco);
                            _crystal.DataSourceConnections[0].SetLogon(empresa.UsuarioBanco, empresa.SenhaBanco);
                            //_crystal.SetParameterValue("Cod", item.ItemCode);
                            //_crystal.SetParameterValue("Descricao", item.ItemName);
                            string strCodeBar = $@"{item.ItemCode} {item.CodigoItem}"; // adicionado
                            if (empresa.ContadorLote)
                            {
                                _crystal.SetParameterValue("Codigo", strCodeBar); //alterado (item.CodigoItem)
                                
                                imglote = new PrintPickingTag().PrintQrCode(strCodeBar);
                            }
                            else
                            {
                                _crystal.SetParameterValue("Codigo", item.NumLote);
                                imglote = new PrintPickingTag().PrintQrCode(item.NumLote);  //achei
                            }

                            _crystal.SetParameterValue("CodBar", imglote);

                            _crystal.PrintToPrinter(printerSettings, pSettings, false, PrintLayout);
                            _crystal.Close();
                            File.Delete(imglote);

                        }

                        if (oCompany.InTransaction)
                            oCompany.EndTransaction(BoWfTransOpt.wf_Commit);
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //crReport = null;
                    GC.Collect();
                }
            }
        }

        private void RealizarReimpressao(List<ItensRecebimento> lstdItens, string nome_impressora, Empresa empresa)
        {
            Startup._logFile = new GravarLog(true).Escrever("Inicio realização impressão");
            string connectionString = DBHelper.GetConnectionString(empresa);
            Startup._logFile = new GravarLog(true).Escrever("ConnectionString: " + connectionString);
            new COMCompany(empresa, true);
            Startup._logFile = new GravarLog(true).Escrever("empresa: " + empresa);
            var com = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();
            if (com != null)
            {
                Startup._logFile = new GravarLog(true).Escrever("Empresa : " + com.company.CompanyName);
            }
            else
            {
                Startup._logFile = new GravarLog(true).Escrever("Objeto de empresa não localizado");
                throw new Exception("Objeto de empresa não localizado");
            }
            Company oCompany = com.company;
            Startup._logFile = new GravarLog(true).Escrever("company " + oCompany);
            string ReportPath = GlobalHelper.RelatoriosPath + "/Etiqueta.rpt";
            Startup._logFile = new GravarLog(true).Escrever("Path: " + ReportPath);
            var imglote = "";
            if (System.IO.File.Exists(ReportPath))
            {
                try
                {
                    foreach (var item in lstdItens)
                    {
                        using (var _crystal = new ReportDocument())
                        {
                            _crystal.Load(ReportPath);
                            System.Drawing.Printing.PrintDocument pDoc = new System.Drawing.Printing.PrintDocument();
                            PrintLayoutSettings PrintLayout = new PrintLayoutSettings();
                            System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();

                            _crystal.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                            printerSettings.PrinterName = nome_impressora;
                            System.Drawing.Printing.PageSettings pSettings = new System.Drawing.Printing.PageSettings(printerSettings);
                            _crystal.PrintOptions.DissociatePageSizeAndPrinterPaperSize = true;
                            _crystal.PrintOptions.PrinterDuplex = PrinterDuplex.Simplex;

                            _crystal.SetDatabaseLogon(empresa.UsuarioBanco, empresa.SenhaBanco, empresa.InstanciaBanco, empresa.NomeBanco);
                            _crystal.DataSourceConnections[0].SetConnection(empresa.InstanciaBanco, empresa.NomeBanco, empresa.UsuarioBanco, empresa.SenhaBanco);
                            _crystal.DataSourceConnections[0].SetLogon(empresa.UsuarioBanco, empresa.SenhaBanco);
                            //_crystal.SetParameterValue("Cod", item.ItemCode);
                            //_crystal.SetParameterValue("Descricao", item.ItemName);
                            string strCodeBar = $@"{item.ItemCode} {item.CodigoItem}"; // adicionado
                            if (empresa.ContadorLote)
                            {
                                _crystal.SetParameterValue("Codigo", strCodeBar);
                                imglote = new PrintPickingTag().PrintQrCode(strCodeBar);
                            }
                            else
                            {
                                _crystal.SetParameterValue("Codigo", item.NumLote);
                                imglote = new PrintPickingTag().PrintQrCode(item.NumLote);
                            }

                            _crystal.SetParameterValue("CodBar", imglote);

                            _crystal.PrintToPrinter(printerSettings, pSettings, false, PrintLayout);
                            _crystal.Close();
                            File.Delete(imglote);

                        }
                       
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //crReport = null;
                    GC.Collect();
                }
            }
        }

        //ALTERADO PARA CONTROLE DE LOTE 

        //private void RealizarImpressao(List<ItensRecebimento> lstdItens, string nome_impressora, Empresa empresa)
        //{
        //    string ReportPath = GlobalHelper.RelatoriosPath + "/Etiqueta.rpt";
        //    if (System.IO.File.Exists(ReportPath))
        //    {
        //        try
        //        {
        //            foreach (var item in lstdItens)
        //            {
        //                var qr = new QrCode();
        //                if (!Directory.Exists(qr.Directory))
        //                {
        //                    Directory.CreateDirectory(qr.Directory);
        //                }
        //                string qrcode = qr.Directory + "/Qr_" + item.NumLote + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        //                var img = new QrCode().GenerateQrCode(qr.Width, qr.Height, item.NumLote, ZXing.BarcodeFormat.QR_CODE);
        //                img.Save(qrcode);

        //                using (var _crystal = new ReportDocument())
        //                {
        //                    _crystal.Load(ReportPath);
        //                    System.Drawing.Printing.PrintDocument pDoc = new System.Drawing.Printing.PrintDocument();
        //                    PrintLayoutSettings PrintLayout = new PrintLayoutSettings();
        //                    System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();

        //                    _crystal.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
        //                    printerSettings.PrinterName = nome_impressora;
        //                    System.Drawing.Printing.PageSettings pSettings = new System.Drawing.Printing.PageSettings(printerSettings);
        //                    _crystal.PrintOptions.DissociatePageSizeAndPrinterPaperSize = true;
        //                    _crystal.PrintOptions.PrinterDuplex = PrinterDuplex.Simplex;

        //                    _crystal.SetDatabaseLogon(empresa.UsuarioBanco, empresa.SenhaBanco, empresa.InstanciaBanco, empresa.NomeBanco);
        //                    _crystal.DataSourceConnections[0].SetConnection(empresa.InstanciaBanco, empresa.NomeBanco, empresa.UsuarioBanco, empresa.SenhaBanco);
        //                    _crystal.DataSourceConnections[0].SetLogon(empresa.UsuarioBanco, empresa.SenhaBanco);
        //                    _crystal.SetParameterValue("Cod", item.ItemCode);
        //                    _crystal.SetParameterValue("Descricao", item.ItemName);
        //                    if (empresa.ContadorLote)
        //                    {
        //                        var digito = string.Join(item.NumLote, "-", item.Digito);
        //                        _crystal.SetParameterValue("NumLote", item.NumLote);
        //                    }
        //                    else
        //                    {
        //                        _crystal.SetParameterValue("NumLote", item.NumLote);
        //                    }
        //                    _crystal.SetParameterValue("QrCode", qrcode);
        //                    //_crystal.SetParameterValue("Densidade", item);
        //                    //_crystal.SetParameterValue("Peso", item.Pes);
        //                    //string pdf = string.Format(@"{0}\{1}.pdf", PrintDirectory.Directory, "TAG_" + DateTime.Now.ToString("yyyyMMddHHmmsss"));

        //                    //_crystal.ExportToDisk(ExportFormatType.PortableDocFormat, pdf);
        //                    _crystal.PrintToPrinter(printerSettings, pSettings, false, PrintLayout);
        //                    _crystal.Close();
        //                    File.Delete(qrcode);

        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            //crReport = null;
        //            GC.Collect();
        //        }
        //    }
        //}
        //private void RealizarImpressao(List<Dictionary<string, string>> dataset_impressao, string nome_impressora)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < dataset_impressao.Count; i++)
        //    {
        //        int c = i;
        //        Dictionary<string, string> dataset = dataset_impressao[i];
        //        string itemcode1 = dataset["itemCode"];
        //        string itemname1 = CortaTamanhoTexto(dataset["itemName"]);
        //        string unid_med1 = dataset["unidadeMedida"];
        //        string data_venc1 = dataset["dataVenc"];
        //        string num_lote1 = dataset["numLote"];
        //        string lote_fabricante1 = dataset["loteFabricante"];
        //        string fornecedor1 = dataset["fornecedor"];


        //        // atenção a esta inrecementação
        //        c += 2;

        //        string itemcode2 = c <= dataset_impressao.Count ? dataset_impressao[(c - 1)]["itemCode"] : "";
        //        string itemname2 = c <= dataset_impressao.Count ? CortaTamanhoTexto(dataset_impressao[(c - 1)]["itemName"]) : "";
        //        string unid_med2 = c <= dataset_impressao.Count ? dataset_impressao[(c - 1)]["unidadeMedida"] : "";
        //        string data_venc2 = c <= dataset_impressao.Count ? dataset_impressao[(c - 1)]["dataVenc"] : "";
        //        string num_lote2 = c <= dataset_impressao.Count ? dataset_impressao[(c - 1)]["numLote"] : "";
        //        string lote_fabricante2 = c <= dataset_impressao.Count ? dataset_impressao[(c - 1)]["loteFabricante"] : "";
        //        string fornecedor2 = c <= dataset_impressao.Count ? dataset_impressao[(c - 1)]["fornecedor"] : "";

        //        sb.AppendLine();
        //        sb.AppendLine("I8,A,001");
        //        sb.AppendLine("Q160,019");
        //        sb.AppendLine("q831");
        //        sb.AppendLine("rN");
        //        sb.AppendLine("S4");
        //        sb.AppendLine("D11");
        //        sb.AppendLine("ZT");
        //        sb.AppendLine("JB");
        //        sb.AppendLine("OD");
        //        sb.AppendLine("R76,0");
        //        sb.AppendLine("f100");
        //        sb.AppendLine("N");

        //        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A132,144,2,1,1,1,N,\"{0}\"", data_venc1));
        //        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A471,144,2,1,1,1,N,\"{0}\"", data_venc2));
        //        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A317,102,2,1,1,1,N,\"{0}\"", fornecedor1));
        //        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A316,81,2,1,1,1,N,\"{0}\"", lote_fabricante1));
        //        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A320,125,2,1,1,1,N,\"{0}\"", itemname1));
        //        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A649,81,2,1,1,1,N,\"{0}\"", lote_fabricante2));
        //        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "B306,60,2,1,2,6,30,B,\"{0}\"", num_lote1));
        //        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "B646,62,2,1,2,6,30,B,\"{0}\"", num_lote2));
        //        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A651,101,2,1,1,1,N,\"{0}\"", fornecedor2));
        //        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A651,123,2,1,1,1,N,\"{0}\"", itemname2));
        //        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A320,145,2,1,1,1,N,\"{0}\"", itemcode1));
        //        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A205,144,2,1,1,1,N,\"{0}\"", unid_med1));
        //        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A545,144,2,1,1,1,N,\"{0}\"", unid_med2));
        //        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A652,144,2,1,1,1,N,\"{0}\"", itemcode2));

        //        sb.AppendLine("P1");

        //        i++;
        //    }
        //    var txt = sb.ToString();
        //    RawPrinterHelper.SendStringToPrinter(nome_impressora, sb.ToString());
        //}

        private string CortaTamanhoTexto(string texto, int limite_caracteres = 28)
        {
            return texto.Length > limite_caracteres ? texto.Substring(0, limite_caracteres) : texto;
        }

        #endregion
    }
}
