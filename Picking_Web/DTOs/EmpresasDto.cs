using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Picking_Web.DTOs
{
    public class EmpresasDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        [Display(Name = "Ativo?")]
        public bool Ativo { get; set; }

        [Display(Name = "Licenciado?")]
        public bool Licenciado { get; set; }



        [Display(Name = "Licença SAP")]
        public string LicencaSap { get; set; }

        [Display(Name = "Porta SAP")]
        public string PortaSap { get; set; }

        [Display(Name = "Usuário SAP")]
        public string UsuarioSap { get; set; }

        [Display(Name = "Senha SAP")]
        public string SenhaSap { get; set; }



        [Display(Name = "Tipo de Banco de Dados")]
        public string TipoBanco { get; set; }

        [Display(Name = "Instância Banco de Dados")]
        public string InstanciaBanco { get; set; }

        [Display(Name = "Nome do Banco de Dados")]
        public string NomeBanco { get; set; }

        [Display(Name = "Usuário Banco de Dados")]
        public string UsuarioBanco { get; set; }

        [Display(Name = "Senha Banco de Dados")]
        public string SenhaBanco { get; set; }
    }
}