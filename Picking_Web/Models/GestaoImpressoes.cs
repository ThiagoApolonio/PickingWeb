using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Picking_Web.Models
{
    public class GestaoImpressoes
    {
        public int Id { get; set; }

        [Required]
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }

        [Required]
        [Display(Name = "Local de Impressão")]
        public int LugarImpressaoId { get; set; }

        [Display(Name = "Local de Impressão")]
        public List<object> LugarImpressao { get; set; }

        [Required]
        [Display(Name = "Nome Impressora")]
        public string NomeImpressora { get; set; }
    }
}