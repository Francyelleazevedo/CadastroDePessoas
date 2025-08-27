using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Domain.Interfaces;
using CadastroDePessoas.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CadastroDePessoas.Infra.Repositorios
{
    public class PessoaRepositorio(AppDbContexto contexto) : BaseRepositorio<Pessoa>(contexto), IPessoaRepositorio
    {
        public async Task<bool> CpfExisteAsync(string cpf, Guid? ignorarId = null)
        {
            if (ignorarId.HasValue)
            {
                return await _dbSet.AnyAsync(p => p.Cpf == cpf && p.Id != ignorarId.Value);
            }

            return await _dbSet.AnyAsync(p => p.Cpf == cpf);
        }

        public override async Task<Pessoa> ObterPorIdAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Endereco)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public override async Task<IEnumerable<Pessoa>> ListarAsync()
        {
            return await _dbSet
                .Include(p => p.Endereco)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Pessoa>> ObterAsync(System.Linq.Expressions.Expression<Func<Pessoa, bool>> predicado)
        {
            return await _dbSet
                .Include(p => p.Endereco)
                .Where(predicado)
                .ToListAsync();
        }

        public async Task AtualizarPessoaComEnderecoAsync(Pessoa entidade)
        {
            var pessoaExistente = await _contexto.Pessoas
                .Include(p => p.Endereco)
                .FirstOrDefaultAsync(p => p.Id == entidade.Id);

            if (pessoaExistente == null)
            {
                throw new Exception($"Pessoa não encontrada para atualização");
            }

            _contexto.Entry(pessoaExistente).State = EntityState.Detached;

            _contexto.Update(entidade);

            await _contexto.SaveChangesAsync();
        }
    }
}
