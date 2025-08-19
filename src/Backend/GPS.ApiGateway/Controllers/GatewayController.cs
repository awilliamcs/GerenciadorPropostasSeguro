using Microsoft.AspNetCore.Mvc;

namespace GPS.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GatewayController : ControllerBase
    {
        [HttpGet("health")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult Health()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Gateway = "GPS.ApiGateway",
                Version = "1.0.0"
            });
        }

        [HttpGet("routes")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult GetRoutes()
        {
            var routes = new
            {
                Services = new[]
                {
                    new { Name = "AutenticacaoService", Path = "/api/autenticacao", Description = "Serviço de autenticação e autorização" },
                    new { Name = "PessoaService", Path = "/api/pessoa", Description = "Serviço de gerenciamento de pessoas" },
                    new { Name = "PropostaService", Path = "/api/proposta", Description = "Serviço de gerenciamento de propostas" },
                    new { Name = "ContratacaoService", Path = "/api/contratacao", Description = "Serviço de gerenciamento de contratos" }
                },
                BaseUrl = "http://localhost:5004",
                Documentation = "/swagger"
            };

            return Ok(routes);
        }
    }
}
