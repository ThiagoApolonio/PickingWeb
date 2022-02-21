using Picking_Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Picking_Web.ViewModels
{
    public class FormGestaoEtiquetasViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }
        public IEnumerable<Empresa> Empresas { get; set; }

        [Required]
        [Display(Name = "Usuário")]
        public string UserId { get; set; }

        [Display(Name = "Usuário")]
        public List<ApplicationUser> Users { get; set; }

        [Required]
        [Display(Name = "Nome Impressora")]
        public string NomeImpressoraEtiqueta { get; set; }

        [Display(Name = "IP Balança")]
        public string IP { get; set; }

        [Display(Name = "Porta Balança")]
        public string Porta { get; set; }

        public List<object> Impressoras { get; set; }

        public string Title
        {
            get { return (Id != 0 ? "Editar Gestão de Etiquetas" : "Nova Gestão de Etiquetas"); }
        }
    }
}