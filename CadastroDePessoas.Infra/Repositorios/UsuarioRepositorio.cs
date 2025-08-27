using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Domain.Interfaces;
using CadastroDePessoas.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CadastroDePessoas.Infra.Repositorios
{
    public class UsuarioRepositorio(AppDbContexto contexto) : BaseRepositorio<Usuario>(contexto), IUsuarioRepositorio
    {
        public async Task<Usuario> ObterPorEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
