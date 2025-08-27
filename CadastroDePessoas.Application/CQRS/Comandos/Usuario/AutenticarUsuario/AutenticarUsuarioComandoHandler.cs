using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace CadastroDePessoas.Application.CQRS.Comandos.Usuario.AutenticarUsuario
{
    public class AutenticarUsuarioComandoHandler(IUsuarioRepositorio repositorioUsuario, IServiceToken servicoToken) : IHandlerComando<AutenticarUsuarioComando, string>
    {
        public async Task<string> Handle(AutenticarUsuarioComando comando, CancellationToken cancellationToken)
        {
            var usuario = await repositorioUsuario.ObterPorEmailAsync(comando.Email) ?? throw new Exception("Usuário ou senha inválidos");

            if (!BC.Verify(comando.Senha, usuario.Senha))
                throw new Exception("Usuário ou senha inválidos");

            return servicoToken.GerarToken(usuario.Id, usuario.Email, usuario.Nome);
        }
    }
}
