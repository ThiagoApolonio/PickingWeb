using System.ComponentModel.DataAnnotations;

namespace Picking_Web.Models
{
    public class GestaoEtiquetas
    {
        public int Id { get; set; }

        [Required]
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }

        [Required]
        [Display(Name = "Usuário")]
        public string UserId { get; set; }

        [Display(Name = "Usuários")]
        public ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "Nome Impressora")]
        public string NomeImpressoraEtiqueta { get; set; }


        [Display(Name = "IP Balança")]
        public string IP { get; set; }

        [Display(Name = "Porta Balança")]
        public string Porta { get; set; }
    }
}