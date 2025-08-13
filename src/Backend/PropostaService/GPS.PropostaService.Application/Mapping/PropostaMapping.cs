using AutoMapper;
using GPS.PropostaService.Application.DTOs;
using GPS.PropostaService.Domain.Entidades;

namespace GPS.PropostaService.Application.Mapping
{
    public class PropostaProfile : Profile
    {
        public PropostaProfile()
        {
            CreateMap<PropostaDto, Proposta>()
                .ConstructUsing(dto => new Proposta(dto.IdPessoa, dto.Tipo, dto.Valor))
                .ReverseMap();
        }
    }
}