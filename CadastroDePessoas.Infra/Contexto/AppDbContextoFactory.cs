using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CadastroDePessoas.Infra.Contexto
{
    public class AppDbContextoFactory : IDesignTimeDbContextFactory<AppDbContexto>
    {
        public AppDbContexto CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContexto>();
            
            var connectionString = "Data Source=cadastro_pessoas.db";
            optionsBuilder.UseSqlite(connectionString);

            return new AppDbContexto(optionsBuilder.Options);
        }
    }
}