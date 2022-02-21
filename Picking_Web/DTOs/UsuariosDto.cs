using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Picking_Web.DTOs
{
    public class UsuariosDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }
        public bool Licenciado { get; set; }
        public bool Operador { get; set; }
    }
}