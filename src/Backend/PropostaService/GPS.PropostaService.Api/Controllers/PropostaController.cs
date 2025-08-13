using GPS.PropostaService.Application.DTOs;
using GPS.PropostaService.Application.Extensions;
using GPS.PropostaService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GPS.PropostaService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PropostaController(IPropostaService propostaService) : ControllerBase
    {
        private readonly IPropostaService _propostaService = propostaService;

        [HttpPost]
        public async Task<IActionResult> SalvarAsync(PropostaDto propostaDto)
        {
            try
            {
                var erros = propostaDto.Validar();

                if (erros.Count != 0)
                    return BadRequest(erros);

                await _propostaService.SalvarAsync(propostaDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterAsync([FromQuery] int pagina = 1, [FromQuery] int quantidadeItens = 10)
        {
            try
            {
                var resultado = await _propostaService.ObterAsync(pagina, quantidadeItens);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("{idProposta}")]
        public async Task<IActionResult> ObterPorIdAsync(Guid idProposta)
        {
            try
            {
                var proposta = await _propostaService.ObterPorIdAsync(idProposta);
                
                if (proposta == null)
                    return NotFound("Proposta não encontrada");

                return Ok(proposta);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
