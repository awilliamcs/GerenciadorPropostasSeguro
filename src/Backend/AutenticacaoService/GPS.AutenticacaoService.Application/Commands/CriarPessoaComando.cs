namespace GPS.AutenticacaoService.Application.Commands
{
    public class CriarPessoaComando
    {
        public Guid SagaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
    }
}
