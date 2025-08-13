using MassTransit;

namespace GPS.CrossCutting.Messaging
{
    public class MassTransitMQClient(IBus bus) : IClienteMQ
    {
        public async Task PublicarMensagemParaFila<T>(T mensagem)
        {
            await PublicarMensagemParaFilaEspecifica(mensagem, typeof(T).Name);
        }

        public async Task PublicarMensagemParaFilaEspecifica<T>(T mensagem, string nomeDaFila)
        {
            await (await bus.GetSendEndpoint(new Uri("queue:" + nomeDaFila))).Send((object)mensagem, default(CancellationToken));
        }
    }
}
