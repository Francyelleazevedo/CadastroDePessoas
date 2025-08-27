using CadastroDePessoas.Domain.Interfaces;
using CadastroDePessoas.Infra.Contexto;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CadastroDePessoas.Infra.Repositorios
{
    public class BaseRepositorio<T>(AppDbContexto contexto) : IBaseRepositorio<T> where T : class
    {
        protected readonly AppDbContexto _contexto = contexto;
        protected readonly DbSet<T> _dbSet = contexto.Set<T>();

        public virtual async Task<T> ObterPorIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> ListarAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> ObterAsync(Expression<Func<T, bool>> predicado)
        {
            return await _dbSet.Where(predicado).ToListAsync();
        }

        public async Task AdicionarAsync(T entidade)
        {
            await _dbSet.AddAsync(entidade);
            await _contexto.SaveChangesAsync();
        }

        public async Task AtualizarAsync(T entidade)
        {
            _dbSet.Update(entidade);
            await _contexto.SaveChangesAsync();
        }

        public async Task RemoverAsync(T entidade)
        {
            _dbSet.Remove(entidade);
            await _contexto.SaveChangesAsync();
        }

        public async Task<bool> ExisteAsync(Expression<Func<T, bool>> predicado)
        {
            return await _dbSet.AnyAsync(predicado);
        }
    }
}
