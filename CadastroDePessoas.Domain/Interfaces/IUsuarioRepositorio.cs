using CadastroDePessoas.Domain.Entidades;

namespace CadastroDePessoas.Domain.Interfaces
{
    public interface IUsuarioRepositorio : IBaseRepositorio<Usuario>
    {
        Task<Usuario> ObterPorEmailAsync(string email);
    }
}
