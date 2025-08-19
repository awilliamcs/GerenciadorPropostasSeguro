using GPS.AutenticacaoService.Application.Events;
using GPS.AutenticacaoService.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GPS.AutenticacaoService.Application.Consumers
{
    public class PessoaCriadaEventConsumer(
        IRegistroUsuarioSagaOrchestrator sagaOrchestrator,
        ILogger<PessoaCriadaEventConsumer> logger) : IConsumer<PessoaCriadaEvent>
    {
        private readonly IRegistroUsuarioSagaOrchestrator _sagaOrchestrator = sagaOrchestrator;
        private readonly ILogger<PessoaCriadaEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<PessoaCriadaEvent> context)
        {
            try
            {
                _logger.LogInformation("Evento PessoaCriadaEvent recebido. SagaId: {SagaId}, PessoaId: {PessoaId}", context.Message.SagaId, context.Message.PessoaId);

                await _sagaOrchestrator.ProcessarPessoaCriadaAsync(context.Message, context.CancellationToken);
                
                _logger.LogInformation("Evento PessoaCriadaEvent processado com sucesso. SagaId: {SagaId}", context.Message.SagaId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar PessoaCriadaEvent. SagaId: {SagaId}", context.Message.SagaId);
                throw;
            }
        }
    }
}
