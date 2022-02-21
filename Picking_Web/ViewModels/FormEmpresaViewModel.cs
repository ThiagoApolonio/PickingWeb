using Picking_Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Picking_Web.ViewModels
{
    public class FormEmpresaViewModel
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


        [Display(Name = "Tipo de Banco")]
        [Required]
        public string TiposBancoId { get; set; }


        [Display(Name = "Tipo de Banco")]
        public List<TiposBanco> TiposBanco = new List<TiposBanco>()
        {
            new TiposBanco()
            {
                Id = "6",
                Nome = "MSSQL2008"
            },
            new TiposBanco()
            {
                Id = "7",
                Nome = "MSSQL2012"
            },
            new TiposBanco()
            {
                Id = "8",
                Nome = "MSSQL2014"
            },
            new TiposBanco()
            {
                Id = "11",
                Nome = "MSSQL2017"
            },
            new TiposBanco()
            {
                Id = "9",
                Nome = "HANADB"
            },
        };

        [Display(Name = "Depósitos Lista Picking")]
        public string DepoSapId { get; set; }
        public IEnumerable<DepositosSap> DepositoSAP { get; set; }
        public string[] selectedDeposito { get; set; }


        [Display(Name = "Depósito Padrão Estoque de terceiros")]
        public string DepoPadrao { get; set; }

        // segundos
        [Display(Name = "Atualizar Lista a Cada (milisegundos)")]
        public int Timer { get; set; }
    }
}