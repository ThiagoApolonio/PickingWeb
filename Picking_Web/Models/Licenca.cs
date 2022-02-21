using System.ComponentModel.DataAnnotations;

namespace Picking_Web.Models
{
    public class Licenca
    {
        public int Id { get; set; }

        [Display(Name = "Quantidade de Licenças")]
        public string QuantidadeLicencas { get; set; }
    }
}