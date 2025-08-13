using AutoMapper;
using GPS.PessoaService.Application.DTOs;
using GPS.PessoaService.Domain.Entidades;

namespace GPS.PessoaService.Application.Mapping
{
    public class PessoaProfile : Profile
    {
        public PessoaProfile()
        {
            CreateMap<PessoaDto, Pessoa>()
                .ConstructUsing(dto => new Pessoa(dto.Nome, dto.Email, dto.Telefone, dto.DataNascimento))
                .ReverseMap();
        }
    }
}