using Picking_Web.Helpers;
using Picking_Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Picking_Web.ViewModels
{
    public class FormGestaoImpressoesViewModel
    {

        public int Id { get; set; }

        [Required]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }
        public IEnumerable<Empresa> Empresas { get; set; }

        [Required]
        [Display(Name = "Local de Impressão")]
        public int LugarImpressaoId { get; set; }

        [Display(Name = "Local de Impressão")]
        public List<object> LugarImpressao = new List<object>()
        {
            new
            {
                Id = GlobalHelper.LugarParaImprimirGeracaoListaPicking,
                Nome = GlobalHelper.LugaresParaImpressao[GlobalHelper.LugarParaImprimirGeracaoListaPicking]
            },
            new
            {
                Id = GlobalHelper.LugarParaImprimirImpressoraAmbiente,
                Nome = GlobalHelper.LugaresParaImpressao[GlobalHelper.LugarParaImprimirImpressoraAmbiente]
            },
            new
            {
                Id = GlobalHelper.LugarParaImprimirImpressoraGeladeira,
                Nome = GlobalHelper.LugaresParaImpressao[GlobalHelper.LugarParaImprimirImpressoraGeladeira]
            },
        };

        [Required]
        [Display(Name = "Nome Impressora")]
        public string NomeImpressora { get; set; }

        public List<object> Impressoras { get; set; }

        public string Title
        {
            get { return (Id != 0 ? "Editar Gestão de Impressões" : "Nova Gestão de Impressões"); }
        }
    }
}