using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Picking_Web.DTOs;
using Picking_Web.Models;

namespace Picking_Web.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.CreateMap<Impressora, ImpressoraDto>();
            Mapper.CreateMap<ImpressoraDto, Impressora>();

            Mapper.CreateMap<ApplicationUser, UsuariosDto>();
            Mapper.CreateMap<UsuariosDto, ApplicationUser>();

            Mapper.CreateMap<Empresa, EmpresasDto>();
            Mapper.CreateMap<EmpresasDto, Empresa>();

            Mapper.CreateMap<TipoImpressora, TipoImpressoraDto>();

            // Dto to Domain
            Mapper.CreateMap<ImpressoraDto, Impressora>()
                .ForMember(i => i.Id, opt => opt.Ignore());
        }
    }
}