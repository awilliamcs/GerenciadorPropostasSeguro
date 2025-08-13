namespace GPS.PessoaService.Domain.Entidades
{
    public class Pessoa(string nome, string email, string telefone, DateTime dataNascimento)
    {
        public Guid IdPessoa { get; private set; } = Guid.NewGuid();
        public string Nome { get; private set; } = nome;
        public string Email { get; private set; } = email;
        public string Telefone { get; private set; } = telefone;
        public DateTime DataNascimento { get; private set; } = dataNascimento;
    }
}
