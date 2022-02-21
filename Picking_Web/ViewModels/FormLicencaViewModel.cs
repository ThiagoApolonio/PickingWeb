using System.ComponentModel.DataAnnotations;

namespace Picking_Web.ViewModels
{
    public class FormLicencaViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Quantidade de Licenças")]
        public int QuantidadeLicenca { get; set; }

        public string Title
        {
            get { return (Id != 0 ? "Editar Licença" : "Nova Licença"); }
        }
    }
}