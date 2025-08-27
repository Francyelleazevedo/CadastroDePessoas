using MediatR;

namespace CadastroDePessoas.Application.CQRS.Comandos.Usuario.CriarUsuario
{
    public class CriarUsuarioComando : IRequest<bool>
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
