using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Picking_Web.Contexto
{
    public class ItensBaixa
    {
        public bool check { get; set; }
        public string ItemCode { get; set; }
        public string Descricao { get; set; }
        public string Deposito { get; set; }
        public string Lote { get; set; }
        public double Quantidade { get; set; }

        public int MyProperty { get; set; }
    }
}