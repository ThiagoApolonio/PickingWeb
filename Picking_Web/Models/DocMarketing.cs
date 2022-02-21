using Picking_Web.Helpers;
using System;

namespace Picking_Web.Models
{
    public class DocMarketing
    {
        public int NumDoc { get; set; }
        public int NumPk { get; set; }
        public string LocalFisico { get; set; }

        public string _prioridade;
        public string Prioridade
        {
            get
            {
                string res = "";
                switch (_prioridade)
                {
                    case "S":
                        res = "P";
                        break;
                    case "N":
                        res = "N/P";
                        break;
                    default:
                        res = "N/P";
                        break;
                }
                return res;
            }
            set { this._prioridade = value; }
        }

        public string NomeCliente { get; set; }

        private string _statuspicking;
        public string StatusPicking
        {
            get
            {
                return this._statuspicking;
            }
            set { this._statuspicking = value; }
        }

        public string Cidade { get; set; }

        private DateTime _dataentrega;
        public string DataEntrega
        {
            get { return DateHelper.ConvertToPTBR(_dataentrega); }
            set { this._dataentrega = DateHelper.GetFromDB(value); }
        }

        public string HoraEntrega { get; set; }

        public string Vendedor { get; set; }

        public string Operador { get; set; }

        public string OperadorId { get; set; }

        public string Observacoes { get; set; }

        private DateTime _datetimeinicio;
        public string DataInicio
        {
            get { return _datetimeinicio == DateTime.MinValue ? "--" : DateHelper.ConvertToPTBR(_datetimeinicio); }
            set { this._datetimeinicio = String.IsNullOrEmpty(value) ? DateTime.MinValue : DateHelper.GetFromDB(value); }
        }

        public string HoraInicio { get; set; }

        private int _objtype = 0;
        public string Tipo
        {
            get
            {
                switch (this._objtype)
                {
                    case ((int)SAPbobsCOM.BoObjectTypes.oOrders):
                        return "PV";
                        break;
                    case ((int)SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest):
                        return "TRF";
                        break;
                    case ((int)SAPbobsCOM.BoObjectTypes.oDeliveryNotes):
                        return "ENT";
                        break;
                    case ((int)SAPbobsCOM.BoObjectTypes.oProductionOrders):
                        return "OP";
                        break;
                    default:
                        return "PV";
                        break;
                }
            }
            set { this._objtype = NumberHelper.GetFromDBToInt(value); }
        }


    }
}