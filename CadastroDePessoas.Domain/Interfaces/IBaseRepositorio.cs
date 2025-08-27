using System.Linq.Expressions;

namespace CadastroDePessoas.Domain.Interfaces
{
    public interface IBaseRepositorio<T> where T : class
    {
        Task<T> ObterPorIdAsync(Guid id);
        Task<IEnumerable<T>> ListarAsync();
        Task<IEnumerable<T>> ObterAsync(Expression<Func<T, bool>> predicado);
        Task AdicionarAsync(T entidade);
        Task AtualizarAsync(T entidade);
        Task RemoverAsync(T entidade);
        Task<bool> ExisteAsync(Expression<Func<T, bool>> predicado);
    }
}
