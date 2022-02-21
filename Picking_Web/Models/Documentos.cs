using AutoMapper;
using Picking_Web.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Picking_Web.Models
{
    public class Documentos
    {
        private ApplicationDbContext _context;
        public Documentos() {
            _context = new ApplicationDbContext();
            Empresas = _context.Empresa.ToList().Select(Mapper.Map<Empresa, EmpresasDto>);
            ListaEmpresas = new List<ListaEmpresas>();
            var empresas = new ListaEmpresas();
            foreach (var item in Empresas)
            {
                empresas.Id = item.Id;
                empresas.Nome = item.Nome;
                ListaEmpresas.Add(empresas);
                empresas = new ListaEmpresas();
            }

            Status = new List<StatusDocumento>();
            var status = new StatusDocumento();

            status.Id = "AS";
            status.Value = "Aguardando Separação - (AS)";
            Status.Add(status);

            status = new StatusDocumento();
            status.Id = "ES";
            status.Value = "Em Separação - (ES)";
            Status.Add(status);

            status = new StatusDocumento();
            status.Id = "AC";
            status.Value = "Aguardando Conferencia - (AC)";
            Status.Add(status);

            status = new StatusDocumento();
            status.Id = "EC";
            status.Value = "Em Conferência - (EC)";
            Status.Add(status);

            status = new StatusDocumento();
            status.Id = "AP";
            status.Value = "Aguardando Peso e Rotulo - AP";
            Status.Add(status);

            status = new StatusDocumento();
            status.Id = "EP";
            status.Value = "Em Peso e Rotulo - (EP)";
            Status.Add(status);

            status = new StatusDocumento();
            status.Id = "PE";
            status.Value = "Picking Efetuado - (PE)";
            Status.Add(status);

            status = new StatusDocumento();
            status.Id = "SP";
            status.Value = "Separação Pendente - (SP)";
            Status.Add(status);
        }
        public string NDocumento { get; set; }
        public List<StatusDocumento> Status { get; set; }
        public List<ListaEmpresas> ListaEmpresas { get; set; }
        public IEnumerable<EmpresasDto> Empresas { get; set; }
    }
}