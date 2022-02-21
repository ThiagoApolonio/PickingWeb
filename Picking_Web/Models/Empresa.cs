using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Picking_Web.Models
{
    public class Empresa
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Display(Name = "Ativo?")]
        public bool Ativo { get; set; }

        [Display(Name = "Controle de Lote?")]
        public bool ContadorLote { get; set; }

        [Display(Name = "Licenciado?")]
        public bool Licenciado { get; set; }



        [Display(Name = "Licença SAP")]
        public string LicencaSap { get; set; }

        [Display(Name = "Porta SAP")]
        [Required]
        public string PortaSap { get; set; }

        [Display(Name = "Usuário SAP")]
        [Required]
        public string UsuarioSap { get; set; }

        [Display(Name = "Senha SAP")]
        [Required]
        public string SenhaSap { get; set; }



        [Display(Name = "Tipo de Banco de Dados")]
        [Required]
        public string TipoBanco { get; set; }

        [Display(Name = "Instância Banco de Dados")]
        [Required]
        public string InstanciaBanco { get; set; }

        [Display(Name = "Nome do Banco de Dados")]
        [Required]
        public string NomeBanco { get; set; }

        [Display(Name = "Usuário Banco de Dados")]
        [Required]
        public string UsuarioBanco { get; set; }

        [Display(Name = "Senha Banco de Dados")]
        [Required]
        public string SenhaBanco { get; set; }

        [Display(Name = "Depósitos Lista Picking")]
        public string DepoSapId { get; set; }
        public IEnumerable<DepositosSap> DepositoSAP { get; set; }

        [Display(Name = "Depósito Padrão")]
        public string DepoPadrao { get; set; }

        // segundos
        [Display(Name = "Atualizar Lista a Cada (segundos)")]
        public int Timer { get; set; }
    }
}