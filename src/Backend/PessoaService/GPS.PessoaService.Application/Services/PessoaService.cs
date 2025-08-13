using AutoMapper;
using GPS.CrossCutting.Messaging;
using GPS.PessoaService.Application.DTOs;
using GPS.PessoaService.Application.Interfaces;
using GPS.PessoaService.Domain.Interfaces;

namespace GPS.PessoaService.Application.Services
{
    public class PessoaService(IPessoaRepository pessoaRepository, IMapper mapper, IClienteMQ clientMQ) : IPessoaService
    {
        private readonly IPessoaRepository _pessoaRepository = pessoaRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IClienteMQ _clientMQ = clientMQ;

        public async Task SalvarAsync(PessoaDto pessoaDto, CancellationToken ct = default)
        {
            await _clientMQ.PublicarMensagemParaFila<PessoaDto>(pessoaDto);
        }

        public async Task<PessoaDto?> ObterPorIdAsync(Guid idPessoa, CancellationToken ct = default)
        {
            var pessoa = await _pessoaRepository.ObterPorIdAsync(idPessoa, ct);
            return pessoa != null ? _mapper.Map<PessoaDto>(pessoa) : null;
        }
    }
}
