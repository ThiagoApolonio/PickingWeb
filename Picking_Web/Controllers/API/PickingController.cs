using Picking_Web.COMObjects;
using Picking_Web.Helpers;
using Picking_Web.Models;
using CrystalDecisions.Shared;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Picking_Web.Contexto;
using CrystalDecisions.CrystalReports.Engine;

namespace Picking_Web.Controllers.API
{
    public class PickingController : ApiController
    {
        public string usuariotexto;
        #region :: Propriedades e Construtor


        private ApplicationDbContext _context;

        public PickingController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        #endregion


        #region :: API's da Tela 'Lista de Picking'

        [HttpGet]
        public IHttpActionResult ListaPedidosEmAberto(int empresa_id, string numdoc = "")
        {
            List<DocMarketing> res = new List<DocMarketing>() { };
            try
            {
                string campo_data = DateHelper.DateFieldToCompare("tb0.DocDueDate");
                string valor_data = DateHelper.DateFieldToCompare("GETDATE()");
                string condicao_numdoc = !String.IsNullOrEmpty(numdoc) ? "AND tb0.DocEntry = " + numdoc : "";
                string sql = SQLListaPicking(
                    $@"     AND {campo_data} = {valor_data}
                            AND(
                               tb0.U_UPD_PCK_STATUS = 'N'
                                OR tb0.U_UPD_PCK_STATUS = 'AS'
                                OR tb0.U_UPD_PCK_STATUS IS NULL 
                            )
                            {condicao_numdoc} "
                ,empresa_id);
                res = ListParaListaPicking(sql, empresa_id, numdoc);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(res);
        }

        [HttpGet]
        public IHttpActionResult ListaPedidosEmSeparacaoFutura(int empresa_id, string numdoc = "")
        {
            List<DocMarketing> res = new List<DocMarketing>() { };
            try
            {
                string campo_data = DateHelper.DateFieldToCompare("tb0.DocDueDate");
                string valor_data = DateHelper.DateFieldToCompare("GETDATE()");
                string condicao_numdoc = !String.IsNullOrEmpty(numdoc) ? "AND tb0.DocEntry = " + numdoc : "";
                string sql = SQLListaPicking(
                    $@"     AND {campo_data} > {valor_data}
                            AND(
                               tb0.U_UPD_PCK_STATUS = 'N'
                                OR tb0.U_UPD_PCK_STATUS = 'AS'
                                OR tb0.U_UPD_PCK_STATUS IS NULL 
                            )
                            {condicao_numdoc} "
                ,empresa_id);
                res = ListParaListaPicking(sql, empresa_id, numdoc);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(res);
        }

        [HttpGet]
        public IHttpActionResult ListaPedidosPendentes(int empresa_id, string numdoc = "")
        {
            List<DocMarketing> res = new List<DocMarketing>() { };
            try
            {
                string campo_data = DateHelper.DateFieldToCompare("tb0.DocDueDate");
                string valor_data = DateHelper.DateFieldToCompare("GETDATE()");
                string condicao_numdoc = !String.IsNullOrEmpty(numdoc) ? "AND tb0.DocEntry = " + numdoc : "";

                string sql = SQLListaPicking(
                    $@"AND ( 
                            ( 
                                {campo_data} < {valor_data}
                                AND
                                (
                                    tb0.U_UPD_PCK_STATUS = 'N' 
                                    OR tb0.U_UPD_PCK_STATUS = 'AS' 
                                    OR tb0.U_UPD_PCK_STATUS IS NULL
                                ) 
                            ) 
                           OR tb0.U_UPD_PCK_STATUS = 'SP'
                        ) 
                    {condicao_numdoc} "
                ,
                    empresa_id,$@"ORDER BY ISNULL(tb0.U_UPD_PCK_STATUS, 'N') DESC, tb0.DocDueDate ASC, tb0.DocTime ASC");
                res = ListParaListaPicking(sql, empresa_id, numdoc);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(res);
        }

        [HttpGet]
        public IHttpActionResult ListaTransferencias(int empresa_id, string numdoc = "")
        {
            List<DocMarketing> res = new List<DocMarketing>() { };
            try
            {

                string sql = $@"SELECT 
							ObjType as Tipo
							, tb0.{ConfigurationManager.AppSettings["CAMPO_PRIORIDADE"]} as Prioridade 
                            , '' as StatusPicking
							, tb0.DocEntry as NumDoc
							, CASE WHEN tb0.CardName IS NOT NULL THEN tb0.CardName ELSE '--' END as NomeCliente
							, {DateHelper.ConvertToSelectAsDate("tb0.DocDueDate")} as DataEntrega 
							, CASE WHEN tb0.Comments IS NULL THEN '--' ELSE tb0.Comments END as Observacoes
							, '--' as Cidade
							, '--' as Vendedor
                            , '--' as DocTime
							, tb1.AbsEntry
						FROM OWTQ tb0
						LEFT JOIN PKL1 tb1 ON(tb1.OrderEntry = tb0.DocEntry AND tb1.BaseObject = tb0.ObjType AND tb1.PickStatus = 'R')
						WHERE 1 = 1
							AND tb0.DocStatus = 'O'
							AND tb1.AbsEntry IS NULL" +
                            (!String.IsNullOrEmpty(numdoc) ? "AND tb0.DocEntry = " + numdoc : "") +
                        $@" 
                        UNION ALL                        

                        SELECT 
							202 as Tipo
							, NULL as Prioridade 
                            , '' as StatusPicking
							, tb0.DocEntry as NumDoc
							, '--' as NomeCliente
							, {DateHelper.ConvertToSelectAsDate("tb0.DueDate")} as DataEntrega 
							, CASE WHEN tb0.Comments IS NULL THEN '--' ELSE tb0.Comments END as Observacoes
							, '--' as Cidade
							, '--' as Vendedor
                            , '--' as DocTime
							, tb1.AbsEntry
						FROM OWOR tb0
						LEFT JOIN PKL1 tb1 ON(tb1.OrderEntry = tb0.DocEntry AND tb1.BaseObject = 202 AND tb1.PickStatus = 'R')
						WHERE 1 = 1
							AND tb0.Status IN ('P','R')
							AND tb1.AbsEntry IS NULL" +
                            (!String.IsNullOrEmpty(numdoc) ? "AND tb0.DocEntry = " + numdoc : "") +
                        $@" ORDER BY Prioridade DESC, DataEntrega DESC, tb0.DocEntry DESC ";
                res = ListParaListaPicking(sql, empresa_id, numdoc);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(res);
        }

        [HttpGet]
        public IHttpActionResult DetalheListaPicking(int empresa_id, int numdoc)
        {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

            if (empresa == null)
            {
                return NotFound();
            }

            int numPicking = 0;
            string dataPicking = DateHelper.ConvertToPTBR(DateTime.Now);
            string usuario = User.Identity.Name;
            var operadores = GlobalHelper.GetOperadores(_context, empresa.Id, Privilegios.PodeGerenciarListaPickingID);

            string connectionString = DBHelper.GetConnectionString(empresa);
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string sql = @"SELECT isnull((MAX(AbsEntry) + 1), 1) as numPicking FROM OPKL";
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        command.CommandText = sql;
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            numPicking = NumberHelper.GetFromDBToInt(reader["numPicking"]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message + e.StackTrace);
            }

            object obj = new
            {
                numPicking = numPicking,
                dataPicking = dataPicking,
                usuario = usuario,
                operadores = operadores,
                observacoes = ""
            };
            return Ok(obj);
        }


        //// [HttpGet]
        //// public IHttpActionResult DetalheListaPickingParcial(int empresa_id, int numdoc) {
        ////     Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

        ////     if (empresa == null) {
        ////         return NotFound();
        ////     }
        ////     int numPicking = 0;
        ////     string dataPicking = DateHelper.ConvertToPTBR(DateTime.Now);
        ////     string usuario = User.Identity.Name;
        ////     var operadores = GlobalHelper.GetOperadores(_context, empresa.Id, Privilegios.PodeGerenciarListaPickingID);
        ////     List<object> itens = new List<object>() { };

        ////     string connectionString = DBHelper.GetConnectionString(empresa);
        ////     try {
        ////         using (SqlConnection conn = new SqlConnection(connectionString)) {
        ////             if (conn.State == ConnectionState.Closed) {
        ////                 conn.Open();
        ////             }

        ////             string sql = @"SELECT isnull((MAX(AbsEntry) + 1), 1) as numPicking FROM OPKL";
        ////             using (SqlCommand command = new SqlCommand("", conn)) {
        ////                 command.CommandText = sql;
        ////                 SqlDataReader reader = command.ExecuteReader();
        ////                 if (reader.Read()) {
        ////                     numPicking = NumberHelper.GetFromDBToInt(reader["numPicking"]);
        ////                 }
        ////             }
        ////         }


        ////         using (SqlConnection conn2 = new SqlConnection(connectionString)) {
        ////             if (conn2.State == ConnectionState.Closed) {
        ////                 conn2.Open();
        ////             }
        ////             using (SqlCommand command = new SqlCommand("", conn2)) {

        ////                 string sql = string.Format(
        ////                     $@"select  T0.ApplyEntry 'DocEntry',
        ////                     T0.ItemCode,
        ////               Left(T3.Dscription, 35)'Dscription',
        ////                     T0.LocCode'WhsCode',
        ////               T3.Quantity'QtdSolicitada',
        ////                     --T0.DocLine 'LineNum',
        ////                     T2.AbsEntry 'LoteId',
        ////                     T2.DistNumber'LoteNumber',
        ////                     Sum(T1.AllocQty) 'QtdLote',
        ////                     isnull((select sum(T11.RelQtty) + sum(T11.PickQtty)
        ////from PKL1 T10 
        ////inner join PKL2 T11 on T10.AbsEntry = T11.AbsEntry and T10.PickEntry = T11.PickEntry
        ////where 1=1
        ////and T10.OrderEntry = T0.ApplyEntry 
        ////and T10.PickStatus <> 'C' 
        ////and T11.SnBEntry = T2.AbsEntry), 0) 'QtdPicking'
        ////                     from OITL T0
        ////                     left Join ITL1 T1 on T0.LogEntry = T1.LogEntry --and T0.LogEntry = T1.MdAbsEntry
        ////                     left Join OBTN T2 on T1.SysNumber = T2.SysNumber and T2.AbsEntry = T1.MdAbsEntry
        ////               inner join RDR1 T3 on T0.ApplyEntry = T3.DocEntry and T0.ItemCode = T3.ItemCode
        ////                     where 1=1
        ////                     and T0.ManagedBy = '10000044'
        ////                     and T0.DocType = 17
        ////                     and T0.ApplyEntry = '{numdoc}'
        ////               --and T0.ItemCode = '1001037'
        ////               --and T0.LocCode = '21MEDA'        
        ////                     Group By T0.ApplyEntry,T0.ItemCode,T0.LocCode, T3.Dscription, T3.Quantity, T0.DocLine,T2.AbsEntry,T2.DistNumber, T3.LineNum
        ////                     Having Sum(T1.AllocQty)>0
        ////                     order by T3.LineNum");
        ////                 command.CommandText = sql;
        ////                 SqlDataReader reader = command.ExecuteReader();
        ////                 int i = 0;
        ////                 while (reader.Read()) {
        ////                     itens.Add(new {
        ////                         index = i,
        ////                         Item = reader["ItemCode"],
        ////                         Descricao = reader["Dscription"],
        ////                         QtdSolicitada = reader["QtdSolicitada"],
        ////                         Deposito = reader["WhsCode"],
        ////                         NumLote = reader["LoteNumber"],
        ////                         QtdAlocada = reader["QtdLote"],
        ////                         QtdAlocadaPk = reader["QtdPicking"],
        ////                         QtdPicking = 0,
        ////                         Check = false,
        ////                     });
        ////                     i++;
        ////                 }
        ////             }
        ////         }


        ////     } catch (Exception e) {
        ////         return BadRequest(e.Message + e.StackTrace);
        ////     }

        ////     object obj = new {
        ////         numPicking = numPicking,
        ////         dataPicking = dataPicking,
        ////         usuario = usuario,
        ////         operadores = operadores,
        ////         observacoes = "",
        ////         itens = itens
        ////     };
        ////     return Ok(obj);
        //// }

        [HttpGet]
        public IHttpActionResult DetalheListaPickingParcial(int empresa_id, int numdoc)
        {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

            if (empresa == null)
            {
                return NotFound();
            }
            int numPicking = 0;
            string dataPicking = DateHelper.ConvertToPTBR(DateTime.Now);
            string usuario = User.Identity.Name;
            var operadores = GlobalHelper.GetOperadores(_context, empresa.Id, Privilegios.PodeGerenciarListaPickingID);
            List<object> itens = new List<object>() { };

            string connectionString = DBHelper.GetConnectionString(empresa);
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string sql = @"SELECT isnull((MAX(AbsEntry) + 1), 1) as numPicking FROM OPKL";
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        command.CommandText = sql;
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            numPicking = NumberHelper.GetFromDBToInt(reader["numPicking"]);
                        }
                    }
                }


                using (SqlConnection conn2 = new SqlConnection(connectionString))
                {
                    if (conn2.State == ConnectionState.Closed)
                    {
                        conn2.Open();
                    }
                    using (SqlCommand command = new SqlCommand("", conn2))
                    {

                        string sql = string.Format(
                            $@"select  T0.ApplyEntry 'DocEntry',
                            T0.ItemCode,
		                    Left(T3.Dscription, 35)'Dscription',
                            T0.LocCode'WhsCode',
		                    T3.Quantity'QtdSolicitada',
                            T0.DocLine 'LineNum',
                            T2.AbsEntry 'LoteId',
                            T2.DistNumber'LoteNumber',
                            sum(T1.AllocQty) 'QtdLote',
                            isnull((select sum(T10.U_QtdPicking)
							from [@UPD_PKL4] T10 
							left join pkl1 T20 on T10.U_PkEntry = T20.AbsEntry and t10.U_OrderLine = t20.OrderLine
                            where 1 = 1
                            and T10.U_OrderEntry = T0.DocEntry
							and T20.PickStatus <> 'C'
                            and T10.Status <> 'C'
                            and T10.U_NumLote = T2.DistNumber)
                            ,0) 'QtdPicking'
                            from OITL T0
                            left Join ITL1 T1 on T0.LogEntry = T1.LogEntry--and T0.LogEntry = T1.MdAbsEntry
                            left Join OBTN T2 on T1.SysNumber = T2.SysNumber and T2.AbsEntry = T1.MdAbsEntry
                            inner join RDR1 T3 on T0.ApplyEntry = T3.DocEntry and T0.DocLine = T3.LineNum
                            where 1 = 1
                            and T0.ManagedBy = '10000044'
                            and T0.DocType = 17
                            and T0.ApplyEntry = {numdoc}
                            --and T0.ItemCode = '1001037'
                            --and T0.LocCode = '21MEDA'
                            Group By T0.ApplyEntry, T0.DocLine,T0.ItemCode,T0.LocCode, T3.Dscription, T3.Quantity,T2.AbsEntry,T2.DistNumber, T0.DocEntry
                            Having sum(T1.AllocQty) > 0
                            order by T0.DocLine");
                        command.CommandText = sql;
                        SqlDataReader reader = command.ExecuteReader();
                        int i = 0;
                        while (reader.Read())
                        {
                            itens.Add(new
                            {
                                index = i,
                                Item = reader["ItemCode"],
                                Descricao = reader["Dscription"],
                                QtdSolicitada = reader["QtdSolicitada"],
                                Deposito = reader["WhsCode"],
                                NumLote = reader["LoteNumber"],
                                LineNum = reader["LineNum"],
                                QtdAlocada = reader["QtdLote"],
                                QtdAlocadaPk = reader["QtdPicking"],
                                QtdPicking = 0,
                                Check = false,
                            });
                            i++;
                        }
                    }
                }


            }
            catch (Exception e)
            {
                return BadRequest(e.Message + e.StackTrace);
            }

            object obj = new
            {
                numPicking = numPicking,
                dataPicking = dataPicking,
                usuario = usuario,
                operadores = operadores,
                observacoes = "",
                itens = itens
            };
            return Ok(obj);
        }

        [HttpGet]
        public IHttpActionResult GetItensPedidoListaParcial(int empresa_id, string numdoc)
        {
            try
            {

                if (String.IsNullOrEmpty(numdoc) || numdoc == "-1")
                {
                    return Ok(new List<string>() { });
                }

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

                if (empresa == null)
                {
                    return NotFound();
                }
                string connectionString = DBHelper.GetConnectionString(empresa);

                List<object> res = new List<object>() { };

                using (SqlConnection conn2 = new SqlConnection(connectionString))
                {
                    if (conn2.State == ConnectionState.Closed)
                    {
                        conn2.Open();
                    }
                    using (SqlCommand command = new SqlCommand("", conn2))
                    {
                        //int pedido_objType = (int)SAPbobsCOM.BoObjectTypes.oOrders;

                        string sql = string.Format(
                            $@"select ItemCode, 
                            Dscription as ItemName,
                            '' numSerie,
                            '' codBarras, 
                            '' CodFabricante,
                            '' numLote,
                            '' qtd,
                            '' qtdTotal
                            from RDR1 where DocEntry = {numdoc}"
                        );
                        command.CommandText = sql;
                        SqlDataReader reader = command.ExecuteReader();
                        int i = 0;
                        while (reader.Read())
                        {
                            res.Add(new
                            {
                                index = i,
                                Item = reader["ItemCode"],
                                Descricao = reader["ItemName"],
                                numSerie = reader["numSerie"],
                                CodigoBarras = reader["codBarras"],
                                CodigoFabricante = reader["CodFabricante"],
                                numLote = reader["numLote"],
                                Quantidade = reader["qtd"],
                                QuantidadeTotal = reader["qtdTotal"],
                            });
                            i++;
                        }
                    }
                }
                return Ok(res);


            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult GerarListaPicking()
        {
            try
            {
                string str_empresa_id = HttpContext.Current.Request.Form.Get("empresa_id");
                string str_numdoc = HttpContext.Current.Request.Form.Get("numdoc");
                string str_operador_id = HttpContext.Current.Request.Form.Get("operador");
                string usuario_logado_nome = HttpContext.Current.Request.Form.Get("usuario_logado_nome");
                string observacoes = HttpContext.Current.Request.Form.Get("observacoes");
                string nome_impressora = HttpContext.Current.Request.Form.Get("nome_impressora");

                int empresa_id = NumberHelper.StringToInt(str_empresa_id);
                int numdoc = NumberHelper.StringToInt(str_numdoc);

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
                if (empresa == null)
                {
                    return NotFound();
                }

                ApplicationUser user = _context.Users.Single(u => u.Id == str_operador_id);
                string nome_operador = user.UserName;

                new COMCompany(empresa, true);
                var com = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();

                Company oCompany = com.company;
                using (var pickListCOM = new COMPickList(oCompany))
                {
                    var oPickLists = pickListCOM._pickList;
                    oPickLists.PickDate = DateTime.Now;
                    oPickLists.Remarks = observacoes;
                    oPickLists.Name = usuario_logado_nome;

                    string connectionString = DBHelper.GetConnectionString(empresa);
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        if (conn.State == ConnectionState.Closed)
                        {
                            conn.Open();
                        }

                        string sql =
                            $@"SELECT 
								tb2.LineNum, tb2.Quantity
						FROM ORDR tb1
						INNER JOIN RDR1 tb2 ON (tb2.DocEntry = tb1.DocEntry)
                        INNER JOIN OITM tb3 ON (tb3.ItemCode = tb2.ItemCode)
						WHERE tb1.DocEntry = {numdoc} AND tb3.InvntItem = 'Y'";
                        using (SqlCommand command = new SqlCommand("", conn))
                        {
                            command.CommandText = sql;
                            SqlDataReader reader = command.ExecuteReader();
                            int i = 0;
                            while (reader.Read())
                            {
                                double qtd_pedido = NumberHelper.GetFromDBToDouble(reader["Quantity"]);

                                oPickLists.Lines.SetCurrentLine(i);
                                oPickLists.Lines.BaseObjectType = ((int)BoObjectTypes.oOrders).ToString();
                                oPickLists.Lines.OrderEntry = numdoc;
                                oPickLists.Lines.OrderRowID = NumberHelper.GetFromDBToInt(reader["LineNum"]);
                                oPickLists.Lines.ReleasedQuantity = qtd_pedido;
                                oPickLists.Lines.Add();
                                i++;
                            }
                        }
                    }

                    var res = oPickLists.Add() != 0;
                    if (res)
                    {
                        return BadRequest(oCompany.GetLastErrorDescription());
                    }
                    else
                    {
                        string novo_status = "ES";
                        string erro = GlobalHelper.AtualizarEtapaPicking(empresa, numdoc, str_operador_id, "", novo_status);
                        if (String.IsNullOrEmpty(erro))
                        {
                            //ImprimirRomaneio(empresa, numdoc, nome_impressora, nome_operador);
                            return Ok();
                        }
                        else
                        {
                            return BadRequest(erro);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //[HttpPost]
        //public IHttpActionResult GerarListaPickingParcial() {

        //    try {
        //        bool pedidoCompleto = true;
        //        int numdoc = int.Parse(HttpContext.Current.Request.Form.Get("numdoc"));
        //        string str_operador_id = HttpContext.Current.Request.Form.Get("operador");
        //        string usuario_logado_nome = HttpContext.Current.Request.Form.Get("usuario_logado_nome");
        //        string observacoes = HttpContext.Current.Request.Form.Get("observacoes");
        //        string nome_impressora = HttpContext.Current.Request.Form.Get("nome_impressora");
        //        var itens = HttpContext.Current.Request.Form.Get("itens");
        //        var itensObj = JsonConvert.DeserializeObject<List<ItensPedido>>(itens);
        //        int empresa_id = int.Parse(HttpContext.Current.Request.Form.Get("empresa_id"));

        //        Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
        //        if (empresa == null) {
        //            return NotFound();
        //        }
        //        new COMCompany(empresa, true);
        //        var com = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();

        //        Company oCompany = com.company;

        //        ApplicationUser user = _context.Users.Single(u => u.Id == str_operador_id);
        //        string nome_operador = user.UserName;

        //        using (var pickListCOM = new COMPickList(oCompany)) {
        //            var oPickLists = pickListCOM._pickList;
        //            //oPickLists.PickDate = DateTime.Now;
        //            //oPickLists.Remarks = observacoes;
        //            //oPickLists.Name = usuario_logado_nome;

        //            int i = 0;
        //            var lstItensFil = itensObj.GroupBy(x => x.item);

        //            foreach (var lsItem in lstItensFil) {

        //                var qtdSol = double.Parse(lsItem.Select(x => x.qtdSolicitada).FirstOrDefault());
        //                var qtdPk = lsItem.Sum(x => double.Parse(string.IsNullOrEmpty(x.qtdPicking) ? "0" : x.qtdPicking));
        //                var qtdAlo = lsItem.Sum(x => double.Parse(string.IsNullOrEmpty(x.qtdAlocadaPk) ? "0" : x.qtdAlocadaPk));


        //                if (pedidoCompleto) {

        //                    if ((qtdSol - (qtdAlo + qtdPk)) > 0) {
        //                        pedidoCompleto = false;
        //                    }

        //                }
        //                if (lsItem.Where(x => x.check).Count() > 0 && qtdPk > 0) {


        //                    if (i > 0)
        //                        oPickLists.Lines.Add();

        //                    oPickLists.Lines.SetCurrentLine(i);
        //                    oPickLists.Lines.OrderRowID = SAPHelper.GetOrderRowItem(empresa, lsItem.Key, numdoc, empresa_id);
        //                    oPickLists.Lines.OrderEntry = numdoc;
        //                    oPickLists.Lines.BaseObjectType = ((int)BoObjectTypes.oOrders).ToString();
        //                    var aux = oPickLists.Lines.ReleasedQuantity = lsItem.Where(x => x.qtdPicking != "" && x.check).Sum(x => double.Parse(x.qtdPicking));
        //                    oPickLists.Lines.ReleasedQuantity = lsItem.Where(x => x.qtdPicking != "" && x.check).Sum(x => double.Parse(x.qtdPicking));
        //                    i++;
        //                }
        //            }

        //            var res = oPickLists.Add() != 0;
        //            var docPk = oCompany.GetNewObjectKey();
        //            if (res) {
        //                return BadRequest(oCompany.GetLastErrorDescription());
        //            } else {
        //                //if (AlocarPosicaoPicking(oCompany, empresa, int.Parse(docPk), numdoc, itensObj)) {
        //                //    //AlocarPickingEfetuado(oCompany, empresa, int.Parse(docPk), itensObj);
        //                if (pedidoCompleto)
        //                {
        //                    bool atPedido = GlobalHelper.AtualizaCampoUsuarioStatusPedidoVenda(empresa, numdoc, "SC");
        //                }
        //                //    //ImprimirRomaneio(empresa, numdoc, nome_impressora);
        //                    return Ok();
        //                //} else {
        //                //    SAPHelper.CancelarListaDePicking(empresa, int.Parse(docPk));
        //                //    return BadRequest("Falha ao alocar lotes no picking.");
        //                //}
        //            }
        //        }

        //    } catch (Exception e) {
        //        return BadRequest(e.Message);
        //    }
        //}
        [HttpPost]
        public IHttpActionResult GerarListaPickingParcial()
        {

            try
            {
                bool ValidQtdPK = true;
                bool pedidoCompleto = true;
                int numdoc = int.Parse(HttpContext.Current.Request.Form.Get("numdoc"));
                string str_operador_id = HttpContext.Current.Request.Form.Get("operador");
                string usuario_logado_nome = HttpContext.Current.Request.Form.Get("usuario_logado_nome");
                string observacoes = HttpContext.Current.Request.Form.Get("observacoes");
                string nome_impressora = HttpContext.Current.Request.Form.Get("nome_impressora");
                var itens = HttpContext.Current.Request.Form.Get("itens");
                var itensObj = JsonConvert.DeserializeObject<List<ItensPedido>>(itens);
                int empresa_id = int.Parse(HttpContext.Current.Request.Form.Get("empresa_id"));

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
                string connectionString = DBHelper.GetConnectionString(empresa);

                if (empresa == null)
                {
                    return NotFound();
                }

                new COMCompany(empresa, true);
         
                var com = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();

                Company oCompany = com.company;

                ApplicationUser user = _context.Users.Single(u => u.Id == str_operador_id);
                string nome_operador = user.UserName;

                using (var pickListCOM = new COMPickList(oCompany))
                {
                    var oPickLists = pickListCOM._pickList;
                    //oPickLists.PickDate = DateTime.Now;
                    //oPickLists.Remarks = observacoes;
                    //oPickLists.Name = usuario_logado_nome;


                    int i = 0;
                    var lstItensFil = itensObj.GroupBy(x => x.LineNum);

                    foreach (var lsItem in lstItensFil)
                    {

                        var qtdSol = double.Parse(lsItem.Select(x => x.qtdSolicitada).FirstOrDefault());
                        var qtdPk = lsItem.Sum(x => double.Parse(string.IsNullOrEmpty(x.qtdPicking) ? "0" : x.qtdPicking));
                        var qtdAlo = lsItem.Sum(x => double.Parse(string.IsNullOrEmpty(x.qtdAlocadaPk) ? "0" : x.qtdAlocadaPk));
                        var qtdAloLt = lsItem.Sum(x => double.Parse(string.IsNullOrEmpty(x.qtdAlocada) ? "0" : x.qtdAlocada));


                        if (pedidoCompleto)
                        {

                            if ((qtdSol - (qtdAlo + qtdPk)) > 0)
                            {
                                pedidoCompleto = false;
                            }

                        }
                        if (ValidQtdPK)
                        {
                            if (qtdPk > qtdAloLt)
                            {
                                ValidQtdPK = false;
                            }

                        }
                        if (lsItem.Where(x => x.check).Count() > 0 && qtdPk > 0)
                        {


                            if (i > 0)
                                oPickLists.Lines.Add();

                            oPickLists.Lines.SetCurrentLine(i);
                            //oPickLists.Lines.OrderRowID = SAPHelper.GetOrderRowItem(empresa, lsItem.Key, numdoc, empresa_id);
                            oPickLists.Lines.OrderRowID = Convert.ToInt32(lsItem.Key);
                            oPickLists.Lines.OrderEntry = numdoc;
                            oPickLists.Lines.BaseObjectType = ((int)BoObjectTypes.oOrders).ToString();
                            var aux = oPickLists.Lines.ReleasedQuantity = lsItem.Where(x => x.qtdPicking != "" && x.check).Sum(x => double.Parse(x.qtdPicking));
                            oPickLists.Lines.ReleasedQuantity = lsItem.Where(x => x.qtdPicking != "" && x.check).Sum(x => double.Parse(x.qtdPicking));
                            i++;
                        }
                    }

                    if (ValidQtdPK)
                    {
                        var res = oPickLists.Add() != 0;
                        var docPk = oCompany.GetNewObjectKey();
                        if (res)
                        {

                            return BadRequest(oCompany.GetLastErrorDescription());

                        }

                        else
                        {
                            new DataBaseFunctions(oCompany).AdcPKL4(itensObj, connectionString);
                            // if (AlocarPosicaoPicking(oCompany, empresa, int.Parse(docPk), numdoc, itensObj)) {
                            //AlocarPickingEfetuado(oCompany, empresa, int.Parse(docPk), itensObj);
                            if (pedidoCompleto)
                            {
                                bool atPedido = GlobalHelper.AtualizaCampoUsuarioStatusPedidoVenda(empresa, numdoc, "SC");
                            }
                            // ImprimirRomaneio(empresa, numdoc, nome_impressora);
                            return Ok();
                            //} else {
                            //    SAPHelper.CancelarListaDePicking(empresa, int.Parse(docPk));
                            //    return BadRequest("Falha ao alocar lotes no picking.");
                            //}
                        }
                    }
                    else
                    {
                        return BadRequest("Quantidade Picking excede a quantidade alocada em Lote");
                    }
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public bool AlocarPosicaoPicking(Company oCompany, Empresa empresa, int docPk, int docPedido, List<ItensPedido> itensObj)
        {
            try
            {
                using (var pickListCOM = new COMPickList(oCompany))
                {
                    var oPickLists = pickListCOM._pickList;
                    oPickLists.GetReleasedAllocation(docPk);
                    var indexItem = 0;
                    var lstItensFil = itensObj.GroupBy(x => x.item);
                    foreach (var lsItem in lstItensFil)
                    {
                        var i = 0;
                        if (lsItem.Where(x => x.check).Count() > 0)
                        {
                            foreach (var item in lsItem.Where(x => x.check))
                            {
                                if (i > 0)
                                {
                                    oPickLists.Lines.BatchNumbers.Add();
                                    oPickLists.Lines.BinAllocations.Add();
                                }
                                if (item.check && int.Parse(item.qtdPicking) != 0)
                                {
                                    oPickLists.Lines.SetCurrentLine(indexItem);

                                    oPickLists.Lines.BatchNumbers.BatchNumber = item.numLote;
                                    oPickLists.Lines.BatchNumbers.Quantity = double.Parse(item.qtdPicking);
                                    oPickLists.Lines.BatchNumbers.BaseLineNumber = indexItem;

                                    oPickLists.Lines.BinAllocations.BinAbsEntry = GlobalHelper.GetBinAbsEntry(empresa, item.deposito, item.item);
                                    oPickLists.Lines.BinAllocations.Quantity = double.Parse(item.qtdPicking);
                                    oPickLists.Lines.BinAllocations.BaseLineNumber = indexItem;
                                    oPickLists.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = i;
                                    i++;
                                }
                            }
                            indexItem++;
                        }
                    }

                    int RetVal = oPickLists.UpdateReleasedAllocation();

                    if (RetVal != 0)
                    {
                        oCompany.GetLastError(out RetVal, out string msg);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void AlocarPickingEfetuado(Company oCompany, Empresa empresa, int docPk, List<ItensPedido> itensObj)
        {
            using (var pickListCOM = new COMPickList(oCompany))
            {
                var oPickLists = pickListCOM._pickList;
                oPickLists.GetByKey(docPk);
                var indexItem = 0;
                var lstItensFil = itensObj.GroupBy(x => x.item);
                foreach (var lsItem in lstItensFil)
                {
                    var i = 0;
                    if (lsItem.Where(x => x.check).Count() > 0)
                    {
                        foreach (var item in lsItem.Where(x => x.check))
                        {
                            if (item.check && int.Parse(item.qtdPicking) != 0)
                            {
                                if (i > 0)
                                {
                                    oPickLists.Lines.BatchNumbers.Add();
                                    oPickLists.Lines.BinAllocations.Add();
                                }

                                oPickLists.Lines.SetCurrentLine(indexItem);
                                oPickLists.Lines.PickedQuantity = double.Parse(item.qtdPicking);

                                oPickLists.Lines.BatchNumbers.SetCurrentLine(i);
                                oPickLists.Lines.BatchNumbers.BatchNumber = item.numLote;
                                oPickLists.Lines.BatchNumbers.Quantity = double.Parse(item.qtdPicking);
                                oPickLists.Lines.BatchNumbers.BaseLineNumber = indexItem;

                                oPickLists.Lines.BinAllocations.SetCurrentLine(i);
                                oPickLists.Lines.BinAllocations.BinAbsEntry = GlobalHelper.GetBinAbsEntry(empresa, item.deposito, item.item);
                                oPickLists.Lines.BinAllocations.Quantity = double.Parse(item.qtdPicking);
                                oPickLists.Lines.BinAllocations.BaseLineNumber = indexItem;
                                oPickLists.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = i;
                                i++;
                            }
                        }
                        indexItem++;
                    }
                }

                int RetVal = oPickLists.Update();

                if (RetVal != 0)
                {
                    oCompany.GetLastError(out RetVal, out string msg);
                }
                else
                {

                }
            }
        }

        public void AlocarPickingEfetuadoCompleto(Company oCompany, Empresa empresa, int docPk, List<ItensPedido> itensObj)
        {
            using (var pickListCOM = new COMPickList(oCompany))
            {
                var oPickLists = pickListCOM._pickList;
                oPickLists.GetByKey(docPk);
                var indexItem = 0;
                var lstItensFil = itensObj.GroupBy(x => x.item);
                foreach (var lsItem in lstItensFil)
                {
                    var i = 0;
                    foreach (var item in lsItem)
                    {

                        if (i > 0)
                        {
                            oPickLists.Lines.BatchNumbers.Add();
                            oPickLists.Lines.BinAllocations.Add();
                        }

                        oPickLists.Lines.SetCurrentLine(indexItem);
                        oPickLists.Lines.PickedQuantity = double.Parse(item.qtdAlocada);

                        oPickLists.Lines.BatchNumbers.SetCurrentLine(i);
                        oPickLists.Lines.BatchNumbers.BatchNumber = item.numLote;
                        oPickLists.Lines.BatchNumbers.Quantity = double.Parse(item.qtdAlocada);
                        oPickLists.Lines.BatchNumbers.BaseLineNumber = indexItem;

                        oPickLists.Lines.BinAllocations.SetCurrentLine(i);
                        oPickLists.Lines.BinAllocations.BinAbsEntry = GlobalHelper.GetBinAbsEntry(empresa, item.deposito, item.item);
                        oPickLists.Lines.BinAllocations.Quantity = double.Parse(item.qtdAlocada);
                        oPickLists.Lines.BinAllocations.BaseLineNumber = indexItem;
                        oPickLists.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = i;
                        i++;

                    }
                    indexItem++;

                }

                int RetVal = oPickLists.Update();

                if (RetVal != 0)
                {
                    oCompany.GetLastError(out RetVal, out string msg);
                }
                else
                {

                }
            }
        }
        //[HttpPost]
        //public IHttpActionResult GerarListaPickingCompleta()
        //{

        //    try
        //    {
        //        bool pedidoCompleto = true;
        //        int numdoc = int.Parse(HttpContext.Current.Request.Form.Get("numdoc"));
        //        string str_operador_id = HttpContext.Current.Request.Form.Get("operador");
        //        string usuario_logado_nome = HttpContext.Current.Request.Form.Get("usuario_logado_nome");
        //        string observacoes = HttpContext.Current.Request.Form.Get("observacoes");
        //        string nome_impressora = HttpContext.Current.Request.Form.Get("nome_impressora");
        //        var itens = HttpContext.Current.Request.Form.Get("itens");
        //        var itensObj = JsonConvert.DeserializeObject<List<ItensPedido>>(itens);
        //        int empresa_id = int.Parse(HttpContext.Current.Request.Form.Get("empresa_id"));

        //        Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
        //        string connectionString = DBHelper.GetConnectionString(empresa);
        //        if (empresa == null)
        //        {
        //            return NotFound();
        //        }
        //        new COMCompany(empresa, true);
        //        var com = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();

        //        Company oCompany = com.company;

        //        ApplicationUser user = _context.Users.Single(u => u.Id == str_operador_id);
        //        string nome_operador = user.UserName;

        //        using (var pickListCOM = new COMPickList(oCompany))
        //        {
        //            var oPickLists = pickListCOM._pickList;
        //            //oPickLists.PickDate = DateTime.Now;
        //            //oPickLists.Remarks = observacoes;
        //            //oPickLists.Name = usuario_logado_nome;

        //            int i = 0;
        //            var lstItensFil = itensObj.GroupBy(x => x.item);

        //            foreach (var lsItem in lstItensFil)
        //            {

        //                var qtdSol = double.Parse(lsItem.Select(x => x.qtdSolicitada).FirstOrDefault());
        //                var qtdAloLot = lsItem.Sum(x => double.Parse(string.IsNullOrEmpty(x.qtdAlocada) ? "0" : x.qtdAlocada));
        //                var qtdPk = lsItem.Sum(x => double.Parse(string.IsNullOrEmpty(x.qtdPicking) ? "0" : x.qtdPicking));
        //                var qtdAloPk = lsItem.Sum(x => double.Parse(string.IsNullOrEmpty(x.qtdAlocadaPk) ? "0" : x.qtdAlocadaPk));
        //                var qtdRestante = qtdAloLot - qtdAloPk;
        //                if (qtdRestante > 0)
        //                {


        //                    if (i > 0)
        //                        oPickLists.Lines.Add();

        //                    oPickLists.Lines.SetCurrentLine(i);
        //                    oPickLists.Lines.OrderRowID = SAPHelper.GetOrderRowItem(empresa, lsItem.Key, numdoc, empresa_id);
        //                    oPickLists.Lines.OrderEntry = numdoc;
        //                    oPickLists.Lines.BaseObjectType = ((int)BoObjectTypes.oOrders).ToString();
        //                    oPickLists.Lines.ReleasedQuantity = qtdRestante;
        //                    i++;
        //                }
        //            }

        //            var res = oPickLists.Add() != 0;
        //            var docPk = oCompany.GetNewObjectKey();
        //            if (res)
        //            {
        //                return BadRequest(oCompany.GetLastErrorDescription());
        //            }
        //            else
        //            {

        //                //if (AlocarPosicaoPickingCompleto(oCompany, empresa, int.Parse(docPk), numdoc, itensObj)) {
        //                //AlocarPickingEfetuadoCompleto(oCompany, empresa, int.Parse(docPk), itensObj);
        //                if (pedidoCompleto)
        //                {
        //                    bool atPedido = GlobalHelper.AtualizaCampoUsuarioStatusPedidoVenda(empresa, numdoc, "SC");

        //                    return Ok();
        //                }
        //                //ImprimirRomaneio(empresa, numdoc, nome_impressora);

        //                /*}*/
        //                else
        //                {
        //                    SAPHelper.CancelarListaDePicking(empresa, int.Parse(docPk));
        //                    return BadRequest("Falha ao alocar lotes no picking.");
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}
        [HttpPost]
        public IHttpActionResult GerarListaPickingCompleta()
        {

            try
            {
                bool pedidoCompleto = true;
                int numdoc = int.Parse(HttpContext.Current.Request.Form.Get("numdoc"));
                string str_operador_id = HttpContext.Current.Request.Form.Get("operador");
                string usuario_logado_nome = HttpContext.Current.Request.Form.Get("usuario_logado_nome");
                string observacoes = HttpContext.Current.Request.Form.Get("observacoes");
                string nome_impressora = HttpContext.Current.Request.Form.Get("nome_impressora");
                var itens = HttpContext.Current.Request.Form.Get("itens");
                var itensObj = JsonConvert.DeserializeObject<List<ItensPedido>>(itens);
                int empresa_id = int.Parse(HttpContext.Current.Request.Form.Get("empresa_id"));

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
                string connectionString = DBHelper.GetConnectionString(empresa);
                if (empresa == null)
                {
                    return NotFound();
                }
                new COMCompany(empresa, true);
                var com = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();

                Company oCompany = com.company;

                ApplicationUser user = _context.Users.Single(u => u.Id == str_operador_id);
                string nome_operador = user.UserName;

                using (var pickListCOM = new COMPickList(oCompany))
                {
                    var oPickLists = pickListCOM._pickList;
                    //oPickLists.PickDate = DateTime.Now;
                    //oPickLists.Remarks = observacoes;
                    //oPickLists.Name = usuario_logado_nome;

                    int i = 0;
                    var lstItensFil = itensObj.GroupBy(x => x.LineNum);
                    //var lstItensFil = itensObj;
                    foreach (var lsItem in lstItensFil)
                    {

                        var qtdSol = double.Parse(lsItem.Select(x => x.qtdSolicitada).FirstOrDefault());
                        var qtdAloLot = lsItem.Sum(x => double.Parse(string.IsNullOrEmpty(x.qtdAlocada) ? "0" : x.qtdAlocada));
                        var qtdPk = lsItem.Sum(x => double.Parse(string.IsNullOrEmpty(x.qtdPicking) ? "0" : x.qtdPicking));
                        var qtdAloPk = lsItem.Sum(x => double.Parse(string.IsNullOrEmpty(x.qtdAlocadaPk) ? "0" : x.qtdAlocadaPk));
                        var qtdRestante = qtdAloLot - qtdAloPk;
                        //var qtdSol = double.Parse(lsItem.qtdSolicitada);
                        //var qtdAloLot = double.Parse(string.IsNullOrEmpty(lsItem.qtdAlocada) ? "0" : lsItem.qtdAlocada);
                        //var qtdPk = double.Parse(string.IsNullOrEmpty(lsItem.qtdPicking) ? "0" : lsItem.qtdPicking);
                        //var qtdAloPk = double.Parse(string.IsNullOrEmpty(lsItem.qtdAlocadaPk) ? "0" : lsItem.qtdAlocadaPk);
                        //var qtdRestante = qtdAloLot - qtdAloPk;


                        if (qtdRestante > 0)
                        {


                            if (i > 0)
                                oPickLists.Lines.Add();

                            oPickLists.Lines.SetCurrentLine(i);
                            var aux = SAPHelper.GetOrderRowItem(empresa, lsItem.Key, numdoc, empresa_id);
                            var aux2 = lsItem.Key;
                            oPickLists.Lines.OrderRowID = Convert.ToInt32(lsItem.Key);
                            oPickLists.Lines.OrderEntry = numdoc;
                            oPickLists.Lines.BaseObjectType = ((int)BoObjectTypes.oOrders).ToString();
                            oPickLists.Lines.ReleasedQuantity = qtdRestante;
                            i++;
                        }
                    }

                    var res = oPickLists.Add() != 0;
                    var docPk = oCompany.GetNewObjectKey();
                    if (res)
                    {
                        return BadRequest(oCompany.GetLastErrorDescription());
                    }
                    else
                    {
                        new DataBaseFunctions(oCompany).AdcPKL4PkCompleto(itensObj, connectionString);

                        //if (AlocarPosicaoPickingCompleto(oCompany, empresa, int.Parse(docPk), numdoc, itensObj)) {
                        //AlocarPickingEfetuadoCompleto(oCompany, empresa, int.Parse(docPk), itensObj);
                        if (pedidoCompleto)
                        {
                            bool atPedido = GlobalHelper.AtualizaCampoUsuarioStatusPedidoVenda(empresa, numdoc, "SC");

                            return Ok();
                        }
                        //ImprimirRomaneio(empresa, numdoc, nome_impressora);

                        /*}*/
                        else
                        {
                            SAPHelper.CancelarListaDePicking(empresa, int.Parse(docPk));
                            return BadRequest("Falha ao alocar lotes no picking.");
                        }
                    }
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public bool AlocarPosicaoPickingCompleto(Company oCompany, Empresa empresa, int docPk, int docPedido, List<ItensPedido> itensObj)
        {
            try
            {
                using (var pickListCOM = new COMPickList(oCompany))
                {
                    var oPickLists = pickListCOM._pickList;
                    oPickLists.GetReleasedAllocation(docPk);
                    var indexItem = 0;
                    var lstItensFil = itensObj.GroupBy(x => x.item);
                    foreach (var lsItem in lstItensFil)
                    {
                        var i = 0;

                        foreach (var item in lsItem)
                        {
                            double qtdAloLot = double.Parse(string.IsNullOrEmpty(item.qtdAlocada) ? "0" : item.qtdAlocada);
                            double qtdAloPk = double.Parse(string.IsNullOrEmpty(item.qtdAlocadaPk) ? "0" : item.qtdAlocadaPk);

                            if (i > 0)
                            {
                                oPickLists.Lines.BatchNumbers.Add();
                                oPickLists.Lines.BinAllocations.Add();
                            }
                            if ((qtdAloLot - qtdAloPk) > 0)
                            {
                                oPickLists.Lines.SetCurrentLine(indexItem);

                                oPickLists.Lines.BatchNumbers.BatchNumber = item.numLote;
                                oPickLists.Lines.BatchNumbers.Quantity = qtdAloLot - qtdAloPk;
                                oPickLists.Lines.BatchNumbers.BaseLineNumber = indexItem;

                                oPickLists.Lines.BinAllocations.BinAbsEntry = GlobalHelper.GetBinAbsEntry(empresa, item.deposito, item.item);
                                oPickLists.Lines.BinAllocations.Quantity = qtdAloLot - qtdAloPk;
                                oPickLists.Lines.BinAllocations.BaseLineNumber = indexItem;
                                oPickLists.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = i;
                                i++;
                            }
                        }
                        indexItem++;

                    }

                    int RetVal = oPickLists.UpdateReleasedAllocation();

                    if (RetVal != 0)
                    {
                        oCompany.GetLastError(out RetVal, out string msg);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        public IHttpActionResult ImpedirListaPicking()
        {
            // DEIXA UMA LISTA DE PICKING COMO PENDENTE
            try
            {
                string str_empresa_id = HttpContext.Current.Request.Form.Get("empresa_id");
                string str_numdoc = HttpContext.Current.Request.Form.Get("numdoc");

                int empresa_id = NumberHelper.StringToInt(str_empresa_id);
                int numdoc = NumberHelper.StringToInt(str_numdoc);

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
                if (empresa == null)
                {
                    return NotFound();
                }

                if (GlobalHelper.AtualizaCampoUsuarioStatusPedidoVenda(empresa, numdoc, "SP"))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Não foi possível atualizar este documento para Pendente");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult DesimpedirListaPicking()
        {
            // DEIXA UMA LISTA DE PICKING COMO PENDENTE
            try
            {
                string str_empresa_id = HttpContext.Current.Request.Form.Get("empresa_id");
                string str_numdoc = HttpContext.Current.Request.Form.Get("numdoc");

                int empresa_id = NumberHelper.StringToInt(str_empresa_id);
                int numdoc = NumberHelper.StringToInt(str_numdoc);

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
                if (empresa == null)
                {
                    return NotFound();
                }

                if (GlobalHelper.AtualizaCampoUsuarioStatusPedidoVenda(empresa, numdoc, "AS"))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Não foi possível atualizar este documento para Aguardando Separação");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion


        #region :: API's da Tela 'Acompanhamento de Picking'

        [HttpGet]
        public IHttpActionResult ListaDocumentosEmSeparacao(int empresa_id, string numdoc = "")
        {
            List<DocMarketing> res = new List<DocMarketing>() { };
            if (!User.IsInRole(Privilegios.PodeGerenciarListaPicking))
            {
                return Ok(res);
            }

            try
            {
                res = ListaDocumentosPk(empresa_id, "ES", numdoc);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(res);
        }

        [HttpGet]
        public IHttpActionResult ListaDocumentosAguardandoConferencia(int empresa_id, string numdoc = "")
        {
            List<DocMarketing> res = new List<DocMarketing>() { };
            if (!User.IsInRole(Privilegios.PodeConferirCodigoBarras))
            {
                return Ok(res);
            }

            try
            {
                res = ListaDocumentosPk(empresa_id, "AC", numdoc);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(res);
        }

        [HttpGet]
        public IHttpActionResult ListaDocumentosEmConferencia(int empresa_id, string numdoc = "")
        {
            List<DocMarketing> res = new List<DocMarketing>() { };
            if (!User.IsInRole(Privilegios.PodeConferirCodigoBarras))
            {
                return Ok(res);
            }

            try
            {
                res = ListaDocumentosPk(empresa_id, "EC", numdoc);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(res);
        }

        [HttpGet]
        public IHttpActionResult ListaDocumentosAguardandoEmbalagens(int empresa_id, string numdoc = "")
        {
            List<DocMarketing> res = new List<DocMarketing>() { };
            if (!User.IsInRole(Privilegios.PodeGerenciarEtiqueta))
            {
                return Ok(res);
            }

            try
            {
                res = ListaDocumentosPk(empresa_id, "AP", numdoc);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(res);
        }

        [HttpGet]
        public IHttpActionResult ListaDocumentosEmEmbalagens(int empresa_id, string numdoc = "")
        {
            List<DocMarketing> res = new List<DocMarketing>() { };
            if (!User.IsInRole(Privilegios.PodeGerenciarEtiqueta))
            {
                return Ok(res);
            }

            try
            {
                res = ListaDocumentosPk(empresa_id, "EP", numdoc);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(res);
        }

        [HttpGet]
        public IHttpActionResult ListaDocumentosEmFaturamento(int empresa_id, string numdoc = "")
        {
            List<DocMarketing> res = new List<DocMarketing>() { };
            if (!User.IsInRole(Privilegios.PodeConferirCodigoBarras))
            {
                return Ok(res);
            }

            try
            {
                res = ListaDocumentosPk(empresa_id, "PE", numdoc);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(res);
        }

        [HttpGet]
        public IHttpActionResult DetalheEncerramentoSeparacao(int empresa_id)
        {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

            if (empresa == null)
            {
                return NotFound();
            }

            var operadores = GlobalHelper.GetOperadores(_context, empresa.Id, Privilegios.PodeGerenciarListaPickingID);

            object obj = new
            {
                operadores = operadores,
            };
            return Ok(obj);
        }

        [HttpPost]
        public IHttpActionResult EncerrarSeparacao()
        {
            try
            {
                string novo_status = "AC";
                string erro = GlobalHelper.AtualizarEtapaPickingListaPicking(_context, novo_status);
                if (String.IsNullOrEmpty(erro))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(erro);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult IniciarConferencia()
        {
            try
            {
                string novo_status = "EC";
                string erro = GlobalHelper.AtualizarEtapaPickingListaPicking(_context, novo_status);
                if (String.IsNullOrEmpty(erro))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(erro);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult TrocarLocalFisico()
        {
            try
            {
                string str_empresa_id = HttpContext.Current.Request.Form.Get("empresa_id");
                string str_numdoc = HttpContext.Current.Request.Form.Get("numdoc");
                string local = HttpContext.Current.Request.Form.Get("local");
                string status = "AP";

                int empresa_id = NumberHelper.StringToInt(str_empresa_id);
                int numdoc = NumberHelper.StringToInt(str_numdoc);

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
                if (empresa == null)
                {
                    return BadRequest("Empresa não encontrada");
                }

                string connectionString = DBHelper.GetConnectionString(empresa);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string sql =
                        $@"DELETE FROM [@UPD_PCK_LOCAL] WHERE U_docentry = '{numdoc}' AND U_status = '{status}'";
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }

                if (GlobalHelper.InsereNovoLocalFisicoPickingDoPedido(empresa, numdoc, local, status))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Erro interno. Não foi possível atualizar o local físico.");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult DetalheInicioEmbalagens(int empresa_id)
        {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

            if (empresa == null)
            {
                return NotFound();
            }

            var operadores = GlobalHelper.GetOperadores(_context, empresa.Id, Privilegios.PodeGerenciarEtiquetaID);

            object obj = new
            {
                operadores = operadores,
            };
            return Ok(obj);
        }

        [HttpGet]
        public IHttpActionResult DetalheInicioConferencia(int empresa_id)
        {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

            if (empresa == null)
            {
                return NotFound();
            }

            var operadores = GlobalHelper.GetOperadores(_context, empresa.Id, Privilegios.PodeConferirCodigoBarrasID);

            object obj = new
            {
                operadores = operadores,
            };
            return Ok(obj);
        }

        [HttpPost]
        public IHttpActionResult IniciarEmbalagens()
        {
            try
            {
                string novo_status = "EP";
                string erro = GlobalHelper.AtualizarEtapaPickingListaPicking(_context, novo_status);
                if (String.IsNullOrEmpty(erro))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(erro);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult CancelarListaPicking()
        {
            try
            {
                string str_empresa_id = HttpContext.Current.Request.Form.Get("empresa_id");
                string str_numdoc = HttpContext.Current.Request.Form.Get("numdoc");
                int numPicking = int.Parse(string.IsNullOrEmpty(HttpContext.Current.Request.Form.Get("numpk")) ? "0" : HttpContext.Current.Request.Form.Get("numpk"));
                int empresa_id = NumberHelper.StringToInt(str_empresa_id);
                int numdoc = NumberHelper.StringToInt(str_numdoc);

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
                if (empresa == null)
                {
                    return NotFound();
                }

                string connectionString = DBHelper.GetConnectionString(empresa);
                if (numPicking <= 0)
                    numPicking = GlobalHelper.RetornaAbsEntryListaPickingDoPedido(connectionString, numdoc);
                if (numPicking > 0)
                {
                    new COMCompany(empresa, true);
                    var com = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();
                    Company oCompany = com.company;
                    using (var pickListCOM = new COMPickList(oCompany))
                    {
                        var oPickLists = pickListCOM._pickList;
                        if (oPickLists.GetByKey(numPicking))
                        {
                            if (oPickLists.Close() != 0)
                            {
                                return BadRequest(oCompany.GetLastErrorDescription());
                            }
                            else
                            {
                                var att = GlobalHelper.AtualizaCampoUsuarioStatusPedidoVenda(empresa, numdoc, "AS");
                                if (att)
                                {
                                    new DataBaseFunctions(oCompany).AtualizaStatus(numPicking, connectionString);
                                    if (empresa.ContadorLote)
                                        AtualizaPKL4ControleLoteCancelamento(connectionString, numPicking, "Y");
                                    return Ok();
                                }
                                else
                                {
                                    return BadRequest("Erro ao atualizar situação/etapa do Pedido de Venda");
                                }
                            }
                        }
                    }

                }
                return BadRequest("Pedido não localizado.");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion


        #region :: API's genéricas

        [HttpGet]
        public IHttpActionResult ReimprimirListaPicking(int empresa_id, int numdoc, int numpk, string nome_impressora)
        {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

            if (empresa == null)
            {
                return NotFound();
            }
            try
            {
                ImprimirRomaneio(empresa, numdoc, numpk, nome_impressora);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        #endregion


        #region :: Funções Auxiliares

        /// <summary>
        /// manda imprimir o romaneio
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numdoc"></param>
        private void ImprimirRomaneio(Empresa empresa, int numdoc, int numpk, string printername)
        {
            Startup._logFile = new GravarLog().Escrever("Inicio Impressão de Romaneio");
            string connectionString = DBHelper.GetConnectionString(empresa);
            var nome_separador = GetOperadorSAP(connectionString, usuariotexto); //mimi
            string ReportPath = GlobalHelper.RelatoriosPath + @"\Picking.rpt";
            Startup._logFile = new GravarLog(true).Escrever("Path: " + ReportPath);
            if (System.IO.File.Exists(ReportPath))
            {
                using (var _crystal = new ReportDocument())
                {
                    _crystal.Load(ReportPath);
                    System.Drawing.Printing.PrintDocument pDoc = new System.Drawing.Printing.PrintDocument();
                    PrintLayoutSettings PrintLayout = new PrintLayoutSettings();
                    System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();

                    _crystal.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                    printerSettings.PrinterName = printername;
                    System.Drawing.Printing.PageSettings pSettings = new System.Drawing.Printing.PageSettings(printerSettings);
                    _crystal.PrintOptions.DissociatePageSizeAndPrinterPaperSize = true;
                    _crystal.PrintOptions.PrinterDuplex = PrinterDuplex.Simplex;

                    _crystal.SetDatabaseLogon(empresa.UsuarioBanco, empresa.SenhaBanco, empresa.InstanciaBanco, empresa.NomeBanco);
                    _crystal.DataSourceConnections[0].SetConnection(empresa.InstanciaBanco, empresa.NomeBanco, empresa.UsuarioBanco, empresa.SenhaBanco);
                    _crystal.DataSourceConnections[0].SetLogon(empresa.UsuarioBanco, empresa.SenhaBanco);

                    _crystal.SetParameterValue("DocKey@", numpk);
                    _crystal.SetParameterValue("Separador", nome_separador);


                    _crystal.PrintToPrinter(printerSettings, pSettings, false, PrintLayout);
                    _crystal.Close();


                }
            }
            else
            {
                Startup._logFile = new GravarLog(true).Escrever("Relatorio nao localizado no caminho: " + ReportPath);
            }

            #region Teste
           
            //if (System.IO.File.Exists(ReportPath))
            //{
            //    try
            //    {
            //        CrystalDecisions.CrystalReports.Engine.ReportDocument _crystal = null;
            //        _crystal = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //        _crystal.Load(ReportPath);

            //        System.Drawing.Printing.PrintDocument pDoc = new System.Drawing.Printing.PrintDocument();
            //        CrystalDecisions.Shared.PrintLayoutSettings PrintLayout = new CrystalDecisions.Shared.PrintLayoutSettings();
            //        System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();

            //        _crystal.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
            //        printerSettings.PrinterName = printername;
            //        System.Drawing.Printing.PageSettings pSettings = new System.Drawing.Printing.PageSettings(printerSettings);
            //        _crystal.PrintOptions.DissociatePageSizeAndPrinterPaperSize = true;
            //        _crystal.PrintOptions.PrinterDuplex = PrinterDuplex.Simplex;

            //        Startup._logFile = new GravarLog(true).Escrever("Configurações de impressoras aplicadas.");
            //        _crystal.SetParameterValue("DocKey@", numdoc);
            //        _crystal.SetParameterValue("Separador", nome_separador);
            //        _crystal.DataSourceConnections[0].SetConnection(empresa.InstanciaBanco, empresa.NomeBanco, empresa.UsuarioBanco, empresa.SenhaBanco);
            //        _crystal.DataSourceConnections[0].IntegratedSecurity = false;
            //        _crystal.DataSourceConnections[0].SetLogon(empresa.UsuarioBanco, empresa.SenhaBanco);

            //        Startup._logFile = new GravarLog(true).Escrever("Configurações da empresa aplicadas.");
            //        _crystal.PrintToPrinter(printerSettings, pSettings, false, PrintLayout);

            //        //_crystal.PrintToPrinter(printerSettings, new PageSettings(), false);

            //        _crystal.Close();

            //    }
            //    catch (Exception ex)
            //    {
            //        Startup._logFile = new GravarLog(true).Escrever("ERRO::::==>" + ex.Message);
            //        throw ex;
            //    }
            //    finally
            //    {
            //        //crReport = null;
            //        GC.Collect();
            //    }
            //}
            //else
            //{
            //    Startup._logFile = new GravarLog(true).Escrever("Relatorio nao localizado no caminho: " + ReportPath);
            //}
            #endregion
        }

        /// <summary>
        /// Lista genérica de documentos.
        /// </summary>
        /// <param name="empresa_id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private List<DocMarketing> ListaDocumentos(int empresa_id, string status, string numdoc = "", string sql = "")
        {
            List<DocMarketing> res = new List<DocMarketing>();
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

            if (empresa == null)
            {
                return res;
            }

            string campo_prioridade = "U_UPD_PRIORIDADE";
            string campo_status = "U_UPD_PCK_STATUS";

            string campo_docduedate = DateHelper.ConvertToSelectAsDate("tb0.DocDueDate");
            string campo_datainicio = DateHelper.ConvertToSelectAsDate("tb3.U_date");
            string campo_horainicio = DateHelper.ConvertToSelectAsTime("tb3.U_date");
            string adiciona_pesquisa = !String.IsNullOrEmpty(numdoc) ? "AND tb0.DocEntry = " + numdoc : "";

            sql = !String.IsNullOrEmpty(sql)
                ? sql
                :
                    $@"SELECT 
							tb0.ObjType as Tipo
							, tb0.{campo_prioridade} as Prioridade
                            , tb0.{campo_status} as StatusPicking
                            , tb0.DocEntry as NumDoc
                            , tb0.CardName as NomeCliente
                            , {campo_docduedate} as DataEntrega
                            , CASE WHEN tb0.Comments IS NULL THEN '--' ELSE tb0.Comments END as Observacoes
							, tb1.CityS as Cidade
							, CASE WHEN tb0.SlpCode = -1 THEN '--' ELSE tb2.SlpName END as Vendedor
							, {campo_datainicio} as DataInicio
                            , {campo_horainicio} as HoraInicio
                            , tb5.UserName as Operador
							, CASE WHEN tb6.U_local IS NULL THEN '--' ELSE tb6.U_local END as LocalFisico
							, tb5.Id as OperadorId
						FROM ORDR tb0
							INNER JOIN RDR12 tb1 ON (tb1.DocEntry = tb0.DocEntry)
							INNER JOIN OSLP tb2 ON (tb2.SlpCode = tb0.SlpCode)
							INNER JOIN [@UPD_PCK_ETAPA] tb3 ON (tb3.U_docentry = tb1.DocEntry)
							INNER JOIN [@UPD_PCK_OPERADOR] tb4 ON (tb4.U_docentry = tb1.DocEntry )
							INNER JOIN Picking_Web.dbo.AspNetUsers tb5 ON (tb5.Id = tb4.U_operador collate SQL_Latin1_General_CP850_CI_AS )
							LEFT JOIN [@UPD_PCK_LOCAL] tb6 ON (tb6.U_docentry = tb1.DocEntry AND tb6.U_status = '{status}' )
                        WHERE  1 = 1 
							AND tb0.DocStatus = 'O'
							AND tb0.{campo_status} = '{status}'
                            AND tb3.U_status = '{status}'
                            AND tb4.U_status = '{status}'
                            {adiciona_pesquisa}
						ORDER BY tb0.{campo_prioridade} DESC, tb3.U_date ASC "
            ;

            string connectionString = DBHelper.GetConnectionString(empresa);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    command.CommandText = sql;
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new DocMarketing()
                        {
                            NumDoc = NumberHelper.GetFromDBToInt(reader["NumDoc"]),
                            LocalFisico = reader["LocalFisico"].ToString(),
                            Prioridade = reader["Prioridade"].ToString(),
                            NomeCliente = reader["NomeCliente"].ToString(),
                            StatusPicking = reader["StatusPicking"].ToString(),
                            Cidade = reader["Cidade"].ToString(),
                            DataEntrega = DateHelper.GetFromDB(reader["DataEntrega"]),
                            Vendedor = reader["Vendedor"].ToString(),
                            Operador = reader["Operador"].ToString(),
                            OperadorId = reader["OperadorId"].ToString(),
                            Observacoes = reader["Observacoes"].ToString(),
                            DataInicio = DateHelper.GetFromDB(reader["DataInicio"]),
                            HoraInicio = reader["HoraInicio"].ToString(),

                            Tipo = reader["Tipo"].ToString(),
                        });
                    }
                }
            }
            return res;
        }

        private List<DocMarketing> ListaDocumentosPk(int empresa_id, string status, string numdoc = "", string sql = "")
        {
            List<DocMarketing> res = new List<DocMarketing>();
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

            if (empresa == null)
            {
                return res;
            }

            string adiciona_pesquisa = !String.IsNullOrEmpty(numdoc) ? "AND T0.AbsEntry = " + numdoc : "";

            sql = !String.IsNullOrEmpty(sql)
                ? sql
                :
                    $@"SELECT 
							T0.ObjType as Tipo
							, T0.U_UPD_PRIORIDADE as Prioridade
                            , T0.U_UPD_PCK_STATUS as StatusPicking
                            , T2.DocEntry as NumDoc
                            , T0.AbsEntry as NumPk
                            , T2.CardName as NomeCliente
                            , CONVERT(DATE, T2.DocDueDate, 103) as DataEntrega
                            , CASE WHEN CAST(T0.Remarks as nvarchar(max)) IS NULL THEN '--' ELSE CAST(T0.Remarks as nvarchar(max)) END as Observacoes
							, T3.CityS as Cidade
							, CASE WHEN T2.SlpCode = -1 THEN '--' ELSE T4.SlpName END as Vendedor
							, '' as DataInicio
                            , '' as HoraInicio
                            , '' as Operador
							, '' as LocalFisico
							, '' as OperadorId
							FROM OPKL T0
							inner join PKL1 T5 on T0.AbsEntry = T5.AbsEntry
							inner JOIN ORDR T2 on T5.OrderEntry = T2.DocEntry 
							inner JOIN RDR1 T1 on T2.DocEntry = T1.DocEntry 
							inner JOIN RDR12 T3 ON (T3.DocEntry = T2.DocEntry)
							inner JOIN OSLP T4 ON (T4.SlpCode = T2.SlpCode)
                        WHERE  1 = 1 
							AND T0.Status <> 'C'
							AND T0.U_UPD_PCK_STATUS = '{status}'
                            {adiciona_pesquisa}
                        group by T0.ObjType, T0.U_UPD_PRIORIDADE, T0.U_UPD_PCK_STATUS, T2.DocEntry, T0.AbsEntry, T2.CardName, T2.DocDueDate, CAST(T0.Remarks as nvarchar(max)), T3.CityS, T2.SlpCode, T4.SlpName
						ORDER BY T0.U_UPD_PRIORIDADE DESC "
            ;

            string connectionString = DBHelper.GetConnectionString(empresa);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    command.CommandText = sql;
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new DocMarketing()
                        {
                            NumDoc = NumberHelper.GetFromDBToInt(reader["NumDoc"]),
                            NumPk = NumberHelper.GetFromDBToInt(reader["NumPk"]),
                            LocalFisico = reader["LocalFisico"].ToString(),
                            Prioridade = reader["Prioridade"].ToString(),
                            NomeCliente = reader["NomeCliente"].ToString(),
                            StatusPicking = reader["StatusPicking"].ToString(),
                            Cidade = reader["Cidade"].ToString(),
                            DataEntrega = DateHelper.GetFromDB(reader["DataEntrega"]),
                            Vendedor = reader["Vendedor"].ToString(),
                            Operador = reader["Operador"].ToString(),
                            OperadorId = reader["OperadorId"].ToString(),
                            Observacoes = reader["Observacoes"].ToString(),
                            DataInicio = DateHelper.GetFromDB(reader["DataInicio"]),
                            HoraInicio = reader["HoraInicio"].ToString(),

                            Tipo = reader["Tipo"].ToString(),
                        });
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="empresa_id"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        private List<DocMarketing> ListaDocumentos(int empresa_id, string sql)
        {
            return ListaDocumentos(empresa_id, "", "", sql);
        }

        /// <summary>
        /// Lista genérica para a tela 'Lista de Picking'.
        /// server para todas as matrizes/tabelas da tela.
        /// MUITO CUIDADO AO REALIZAR UMA ALTERAÇÃO.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="empresa_id"></param>
        /// <param name="numdoc"></param>
        /// <returns></returns>
        private List<DocMarketing> ListParaListaPicking(string sql, int empresa_id, string numdoc = "")
        {
            List<DocMarketing> res = new List<DocMarketing>() { };
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

            if (empresa == null)
            {
                return res;
            }

            string connectionString = DBHelper.GetConnectionString(empresa);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    command.CommandText = sql;
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new DocMarketing()
                        {
                            Tipo = reader["Tipo"].ToString(),
                            Prioridade = reader["Prioridade"].ToString(),
                            NumDoc = NumberHelper.GetFromDBToInt(reader["NumDoc"]),
                            NomeCliente = reader["NomeCliente"].ToString(),
                            DataEntrega = DateHelper.GetFromDB(reader["DataEntrega"]),
                            Observacoes = reader["Observacoes"].ToString(),
                            Cidade = reader["Cidade"].ToString(),
                            Vendedor = reader["Vendedor"].ToString(),
                            HoraEntrega = reader["DocTime"].ToString(),
                            StatusPicking = reader["StatusPicking"].ToString(),
                        });
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// monta o where para cada tipo de lista da tela 'lista de picking'
        /// </summary>
        /// <param name="where_concat">where para ser concatenado. ja existe um erro para esta consulta</param>
        /// <returns></returns>
        //private string SQLListaPicking(string where_concat, string orderby = "")
        //{
        //    orderby = String.IsNullOrEmpty(orderby)
        //        ? "	ISNULL(tb0." + ConfigurationManager.AppSettings["CAMPO_PRIORIDADE"] + ",'N') DESC, tb0.DocEntry ASC "
        //        : orderby.Replace("ORDER BY", "")
        //    ;

        //    string sql =
        //        "SELECT " +
        //        "		tb0.ObjType as Tipo " +
        //        "		, tb0." + ConfigurationManager.AppSettings["CAMPO_PRIORIDADE"] + " as Prioridade " +
        //        "		, tb0.DocEntry as NumDoc " +
        //        "		, tb0.U_UPD_PCK_STATUS as StatusPicking " +
        //        "		, tb0.CardName as NomeCliente " +
        //        "       , " + DateHelper.ConvertToSelectAsDate("tb0.DocDueDate") + " as DataEntrega" +
        //        "		, CASE WHEN tb0.Comments IS NULL THEN '--' ELSE tb0.Comments END as Observacoes" +
        //        "		, tb1.CityS as Cidade" +
        //        "		, CASE WHEN tb0.SlpCode = -1 THEN '--' ELSE tb2.SlpName END as Vendedor" +
        //        "       , " + DateHelper.ConvertToSelectTimeFieldAsString("tb0.DocTime", "DocTime") +
        //        "    FROM ORDR tb0 " +
        //        "        INNER JOIN RDR12 tb1 ON(tb1.DocEntry = tb0.DocEntry)" +
        //        "        INNER JOIN OSLP tb2 ON(tb2.SlpCode = tb0.SlpCode)" +
        //        "        INNER JOIN RDR1 tb3 ON(tb3.DocEntry = tb0.DocEntry AND tb3.LineNum = (SELECT TOP(1) LineNum FROM RDR1 t1 WHERE t1.DocEntry = tb0.DocEntry ORDER BY t1.LineNum ASC ) )" +
        //        "        INNER JOIN OITM tb4 ON(tb4.ItemCode = tb3.ItemCode )" +
        //        "    WHERE  1 = 1 " +
        //        "        AND tb0.DocStatus = 'O' AND tb4.ItemClass = 2 " +
        //        where_concat +
        //        " ORDER BY " + orderby;

        //    return sql;
        //}
        private string SQLListaPicking(string where_concat, int empresa_id, string orderby = "")
        {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
            string where_WhsCode = "";
            
            if (!string.IsNullOrEmpty(empresa.DepoSapId))
            {
                string depositos = empresa.DepoSapId.Replace(",","','");
                where_WhsCode = $@"AND (tb3.WhsCode IN ('{depositos}'))";
            }
                orderby = String.IsNullOrEmpty(orderby)
                ? "	ISNULL(tb0." + ConfigurationManager.AppSettings["CAMPO_PRIORIDADE"] + ",'N') DESC, tb0.DocEntry ASC "
                : orderby.Replace("ORDER BY", "")
            ;

            string sql =
                "SELECT " +
                "		tb0.ObjType as Tipo " +
                "		, tb0." + ConfigurationManager.AppSettings["CAMPO_PRIORIDADE"] + " as Prioridade " +
                "		, tb0.DocEntry as NumDoc " +
                "		, tb0.U_UPD_PCK_STATUS as StatusPicking " +
                "		, tb0.CardName as NomeCliente " +
                "       , " + DateHelper.ConvertToSelectAsDate("tb0.DocDueDate") + " as DataEntrega" +
                "		, CASE WHEN tb0.Comments IS NULL THEN '--' ELSE tb0.Comments END as Observacoes" +
                "		, tb1.CityS as Cidade" +
                "		, CASE WHEN tb0.SlpCode = -1 THEN '--' ELSE tb2.SlpName END as Vendedor" +
                "       , " + DateHelper.ConvertToSelectTimeFieldAsString("tb0.DocTime", "DocTime") +
                "    FROM ORDR tb0 " +
                "        INNER JOIN RDR12 tb1 ON(tb1.DocEntry = tb0.DocEntry)" +
                "        INNER JOIN OSLP tb2 ON(tb2.SlpCode = tb0.SlpCode)" +
                "        INNER JOIN RDR1 tb3 ON(tb3.DocEntry = tb0.DocEntry AND tb3.LineNum = (SELECT TOP(1) LineNum FROM RDR1 t1 WHERE t1.DocEntry = tb0.DocEntry ORDER BY t1.LineNum ASC ) )" +
                "        INNER JOIN OITM tb4 ON(tb4.ItemCode = tb3.ItemCode )" +
                "    WHERE  1 = 1 " +
                "        AND tb0.DocStatus = 'O' AND tb4.ItemClass = 2 " +
                where_WhsCode +
                where_concat +
                " ORDER BY " + orderby;

            return sql;
        }
        public int CalculaDigitoLote(string connectionString, string NumLote, string ItemCode)
        {
            var NovoDigito = 0;
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                if (conn2.State == ConnectionState.Closed)
                {
                    conn2.Open();
                }
                using (SqlCommand command = new SqlCommand("", conn2))
                {
                    // verificar 
                    string sql = string.Format(@" select Max(U_DigControle) as Digito  from [@UPD_LOTE] where U_NumLote = '{0}'", NumLote);

                    command.CommandText = sql;
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var Digito = reader["Digito"].ToString();
                        if (string.IsNullOrEmpty(Digito))
                        {
                            NovoDigito = 0;
                        }
                        else
                        {
                            var doubleDig = Convert.ToDouble(Digito);
                            NovoDigito = Convert.ToInt32((int)doubleDig);
                        }
                    }
                    return NovoDigito + 1;
                }
            }
        }
        public string GetOperadorSAP(string connectionString, string usuariotexto)
        {
            var Operador = "";
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                if (conn2.State == ConnectionState.Closed)
                {
                    conn2.Open();
                }
                using (SqlCommand command = new SqlCommand("", conn2))
                {
                   usuariotexto = User.Identity.Name;
                    // verificar 
                    //mimi
                    string sql = string.Format(@"SELECT UserName from Picking_Web..AspNetUsers where UserName='{0}'", usuariotexto);

                    command.CommandText = sql;
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Operador = reader["UserName"].ToString();

                    }
                    return Operador;
                }
            }
        }

        #region :: Atualiza status  Controle Lote
        public void AtualizaPKL4ControleLoteCancelamento(string connectionString, int PkEntry, string Disponibilidade)
        {
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                if (conn2.State == ConnectionState.Closed)
                {
                    conn2.Open();
                }
                using (SqlCommand command = new SqlCommand("", conn2))
                {
                    string sql = string.Format(@"update [@UPD_Lote] set U_Status = '{0}', U_PkEntry = null where U_PkEntry = '{1}'", Disponibilidade, PkEntry);

                    command.CommandText = sql;
                    SqlDataReader reader = command.ExecuteReader();
                }
            }
        }
        #endregion

        #endregion
    }
}
