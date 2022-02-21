using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Picking_Web.Models
{
    public class TipoImpressora
    {
        public byte Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Descricao { get; set; }
    }
}