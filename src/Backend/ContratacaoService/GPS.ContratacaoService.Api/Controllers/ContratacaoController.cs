using GPS.ContratacaoService.Application.DTOs;
using GPS.ContratacaoService.Application.Extensions;
using GPS.ContratacaoService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GPS.ContratacaoService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContratacaoController(IContratacaoService contratacaoService) : ControllerBase
    {
        private readonly IContratacaoService _contratacaoService = contratacaoService;

        [HttpPost]
        public async Task<IActionResult> SalvarAsync(ContratacaoDto contratacaoDto)
        {
            try
            {
                var erros = contratacaoDto.Validar();

                if (erros.Count != 0)
                    return BadRequest(erros);

                await _contratacaoService.SalvarAsync(contratacaoDto);
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
                var resultado = await _contratacaoService.ObterAsync(pagina, quantidadeItens);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("{idContratacao}")]
        public async Task<IActionResult> ObterPorIdAsync(Guid idContratacao)
        {
            try
            {
                var contratacao = await _contratacaoService.ObterPorIdAsync(idContratacao);
                
                if (contratacao == null)
                    return NotFound("Contratacao não encontrada");

                return Ok(contratacao);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
