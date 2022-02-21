using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Picking_Web.Models;

namespace Picking_Web.DTOs
{
    public class ImpressoraDto
    {
        public int Id { get; set; }

        public byte TipoImpressoraId { get; set; }

        public TipoImpressoraDto TipoImpressora { get; set; }

        [Required]
        [StringLength(200)]
        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        public string IP { get; set; }

        public string Porta { get; set; }

        public string Localizacao { get; set; }
    }
}