using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Picking_Web.Contexto {
    public class ItensConferencia {
        public string ItemCode { get; set; }
        public string Descricao { get; set; }
        public string Deposito { get; set; }
        public string NumSerie { get; set; }
        public string NumLote { get; set; }
        public string OrderLine { get; set; }
        public string CodigoBarras { get; set; }
        public double QtdVerificada { get; set; }
        public double QtdSolicitada { get; set; }
    }
}