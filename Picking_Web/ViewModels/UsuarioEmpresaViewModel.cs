using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Picking_Web.ViewModels
{
    public class UsuarioEmpresaViewModel
    {
        public int EmpresaId { get; set; }
        public string Nome { get; set; }
        public bool Assignado { get; set; }
    }
}