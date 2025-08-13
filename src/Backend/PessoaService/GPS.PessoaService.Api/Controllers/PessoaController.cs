using GPS.PessoaService.Application.DTOs;
using GPS.PessoaService.Application.Extensions;
using GPS.PessoaService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GPS.PessoaService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PessoaController(IPessoaService pessoaService) : ControllerBase
    {
        private readonly IPessoaService _pessoaService = pessoaService;

        [HttpPost]
        public async Task<IActionResult> SalvarAsync(PessoaDto pessoaDto)
        {
            try
            {
                var erros = pessoaDto.Validar();

                if (erros.Count != 0)
                    return BadRequest(erros);

                await _pessoaService.SalvarAsync(pessoaDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("{idPessoa}")]
        public async Task<IActionResult> ObterPorIdAsync(Guid idPessoa)
        {
            try
            {
                var pessoa = await _pessoaService.ObterPorIdAsync(idPessoa);

                if (pessoa == null)
                    return NotFound("Pessoa não encontrada");

                return Ok(pessoa);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
