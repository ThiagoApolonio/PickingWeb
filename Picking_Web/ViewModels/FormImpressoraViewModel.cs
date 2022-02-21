using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Picking_Web.Models;

namespace Picking_Web.ViewModels
{
    public class FormImpressoraViewModel
    {
        public IEnumerable<TipoImpressora> TipoImpressoras { get; set; }
        public Impressora Impressora { get; set; }

        public string Title
        {
            get { return (Impressora != null && Impressora.Id != 0 ? "Editar Impressora" : "Nova Impressora"); }
        }
    }
}