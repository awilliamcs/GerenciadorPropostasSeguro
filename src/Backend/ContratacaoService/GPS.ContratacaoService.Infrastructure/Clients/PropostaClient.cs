using GPS.ContratacaoService.Application.DTOs;
using GPS.ContratacaoService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GPS.ContratacaoService.Infrastructure.Clients
{
    public class PropostaClient(HttpClient httpClient, IConfiguration configuration, ILogger<PropostaClient> logger) : IPropostaClient
    {
        private readonly string _baseUrl = configuration["PropostaService:BaseUrl"] ?? throw new InvalidOperationException("PropostaService:BaseUrl não configurado");

        public async Task<PropostaDto?> ObterPorIdAsync(Guid idProposta, CancellationToken ct = default)
        {
            try
            {
                var response = await httpClient.GetAsync($"{_baseUrl}/api/proposta/{idProposta}", ct);
                
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync(ct);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<PropostaDto>(content, options);
            }
            catch (Exception)
            {
                logger.LogError("Erro ao obter proposta por ID: {IdProposta}", idProposta);
                return null;
            }
        }
    }
}
