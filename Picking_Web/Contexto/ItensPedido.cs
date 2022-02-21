using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Picking_Web.Contexto {
    public class ItensPedido {
        public int index { get; set; }
        public string item { get; set; }
        public string descricao { get; set; }
        public string qtdSolicitada { get; set; }
        public string deposito { get; set; }
        public string numLote { get; set; }
        public string LineNum { get; set; }
        public string qtdAlocada { get; set; }
        public string qtdAlocadaPk { get; set; }
        public string qtdPicking { get; set; }
        public bool check { get; set; }




       
    }
}