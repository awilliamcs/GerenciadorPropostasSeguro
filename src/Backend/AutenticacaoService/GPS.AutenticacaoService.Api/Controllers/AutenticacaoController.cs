using GPS.AutenticacaoService.Application.DTOs;
using GPS.AutenticacaoService.Application.Extensions;
using GPS.AutenticacaoService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GPS.AutenticacaoService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacaoController(IAutenticacaoService autenticacaoService) : ControllerBase
    {
        private readonly IAutenticacaoService _autenticacaoService = autenticacaoService;

        [HttpPost("CriarUsuarioComPessoa")]
        [AllowAnonymous]
        public async Task<IActionResult> CriarUsuarioComPessoa([FromBody] RegisterDto registerDto, CancellationToken ct)
        {
            try
            {
                var erros = registerDto.Validar();

                if (erros.Count != 0)
                    return BadRequest(erros);

                var (success, result, errors) = await _autenticacaoService.CriarUsuarioComPessoaAsync(registerDto, ct);

                if (!success)
                    return BadRequest(errors);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost("EfetuarLogin")]
        [AllowAnonymous]
        public async Task<IActionResult> EfetuarLoginAsync([FromBody] LoginDto loginDto, CancellationToken ct)
        {
            try
            {
                var erros = loginDto.Validar();

                if (erros.Count != 0)
                    return BadRequest(erros);

                var result = await _autenticacaoService.EfetuarLoginAsync(loginDto, ct);

                if (!result.Success)
                    return BadRequest(result.Errors);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
