namespace GPS.AutenticacaoService.Application.Sagas
{
    public enum RegistroUsuarioSagaStatus
    {
        Iniciado,
        PessoaCriada,
        UsuarioCriado,
        Finalizado,
        CompensandoPessoa,
        Falhou
    }

    public class RegistroUsuarioSagaData
    {
        public Guid SagaId { get; set; }
        public RegistroUsuarioSagaStatus Status { get; set; }
        public object DadosRegistro { get; set; } = null!;
        public object? PessoaCriada { get; set; }
        public Guid? UserId { get; set; }
        public List<string> Erros { get; set; } = [];
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }
}
