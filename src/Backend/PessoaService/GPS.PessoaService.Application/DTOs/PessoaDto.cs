namespace GPS.PessoaService.Application.DTOs
{
    public class PessoaDto
    {
        public Guid IdPessoa { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}
