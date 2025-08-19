using GPS.AutenticacaoService.Application.Events;
using GPS.AutenticacaoService.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GPS.AutenticacaoService.Application.Consumers
{
    public class PessoaCriacaoFalhouEventConsumer(
        IRegistroUsuarioSagaOrchestrator sagaOrchestrator,
        ILogger<PessoaCriacaoFalhouEventConsumer> logger) : IConsumer<PessoaCriacaoFalhouEvent>
    {
        private readonly IRegistroUsuarioSagaOrchestrator _sagaOrchestrator = sagaOrchestrator;
        private readonly ILogger<PessoaCriacaoFalhouEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<PessoaCriacaoFalhouEvent> context)
        {
            try
            {
                _logger.LogWarning("Evento PessoaCriacaoFalhouEvent recebido. SagaId: {SagaId}, Erro: {Erro}", context.Message.SagaId, context.Message.Erro);

                await _sagaOrchestrator.ProcessarPessoaCriacaoFalhouAsync(context.Message, context.CancellationToken);

                _logger.LogInformation("Evento PessoaCriacaoFalhouEvent processado. SagaId: {SagaId}", context.Message.SagaId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar PessoaCriacaoFalhouEvent. SagaId: {SagaId}", context.Message.SagaId);
                throw;
            }
        }
    }
}
