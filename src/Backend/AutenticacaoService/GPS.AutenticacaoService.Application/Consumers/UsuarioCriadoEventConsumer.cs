using GPS.AutenticacaoService.Application.Events;
using GPS.AutenticacaoService.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GPS.AutenticacaoService.Application.Consumers
{
    public class UsuarioCriadoEventConsumer(
        IRegistroUsuarioSagaOrchestrator sagaOrchestrator,
        ILogger<UsuarioCriadoEventConsumer> logger) : IConsumer<UsuarioCriadoEvent>
    {
        private readonly IRegistroUsuarioSagaOrchestrator _sagaOrchestrator = sagaOrchestrator;
        private readonly ILogger<UsuarioCriadoEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<UsuarioCriadoEvent> context)
        {
            try
            {
                _logger.LogInformation("Evento UsuarioCriadoEvent recebido. SagaId: {SagaId}, UserId: {UserId}", context.Message.SagaId, context.Message.UserId);

                await _sagaOrchestrator.ProcessarUsuarioCriadoAsync(context.Message, context.CancellationToken);

                _logger.LogInformation("Evento UsuarioCriadoEvent processado com sucesso. SagaId: {SagaId}", context.Message.SagaId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar UsuarioCriadoEvent. SagaId: {SagaId}", context.Message.SagaId);
                throw;
            }
        }
    }
}
