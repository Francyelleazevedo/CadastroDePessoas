using System.Security.Claims;

namespace CadastroDePessoas.Application.Interfaces
{
    public interface IServiceToken
    {
        string GerarToken(Guid usuarioId, string email, string nome, IEnumerable<string> roles = null);
        ClaimsPrincipal ValidarToken(string token);
    }
}
