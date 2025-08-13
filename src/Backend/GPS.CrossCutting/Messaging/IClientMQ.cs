namespace GPS.CrossCutting.Messaging
{
    public interface IClienteMQ
    {
        Task PublicarMensagemParaFila<T>(T mensagem);

        Task PublicarMensagemParaFilaEspecifica<T>(T mensagem, string nomeDaFila);
    }
}
