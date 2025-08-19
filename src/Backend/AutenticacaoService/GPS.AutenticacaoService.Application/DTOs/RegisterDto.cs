namespace GPS.AutenticacaoService.Application.DTOs
{
    public class RegisterDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Senha { get; set; }
    }
}
