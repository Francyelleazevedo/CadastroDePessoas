namespace CadastroDePessoas.Application.Dtos.Usuario
{
    public class CriarUsuarioDTO
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string ConfirmacaoSenha { get; set; }
    }
}
