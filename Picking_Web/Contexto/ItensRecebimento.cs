using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Picking_Web.Contexto {
    public class ItensRecebimento {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnidMed { get; set; }
        public string DataVenc { get; set; }
        public string NumLote { get; set; }
        public int Digito { get; set; }
        public string CodigoItem { get; set; }
        public string Ambiente { get; set; }
        public string Quantidade { get; set; }
        public string LoteFab { get; set; }
        public string Fornecedor { get; set; }

    }
}