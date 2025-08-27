using MediatR;

namespace CadastroDePessoas.Application.CQRS.Comandos.Usuario.AutenticarUsuario
{
    public class AutenticarUsuarioComando : IRequest<string>
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
