using AutoMapper;
using GPS.ContratacaoService.Application.DTOs;
using GPS.ContratacaoService.Domain.Entidades;

namespace GPS.ContratacaoService.Application.Mapping
{
    public class ContratacaoProfile : Profile
    {
        public ContratacaoProfile()
        {
            CreateMap<ContratacaoDto, Contratacao>()
                .ConstructUsing(dto => new Contratacao(dto.IdProposta))
                .ReverseMap();
        }
    }
}