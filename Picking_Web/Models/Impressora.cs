using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Picking_Web.Models
{
    public class Impressora
    {
        public int Id { get; set; }

        public TipoImpressora TipoImpressora { get; set; }

        [Display(Name = "Tipo de Impressora")]
        public byte TipoImpressoraId { get; set; }

        [Required]
        [StringLength(200)]
        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        public string IP { get; set; }

        public string Porta { get; set; }

        public string Localizacao { get; set; }
    }
}