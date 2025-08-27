using CadastroDePessoas.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace CadastroDePessoas.Infra.Contexto
{
    public class AppDbContexto(DbContextOptions<AppDbContexto> options) : DbContext(options)
    {
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContexto).Assembly);

            modelBuilder.Entity<Pessoa>().Ignore("EnderecoCompleto");

            modelBuilder.Entity<Pessoa>()
                .HasOne(p => p.Endereco)
                .WithOne(e => e.Pessoa)
                .HasForeignKey<Endereco>(e => e.PessoaId)
                .OnDelete(DeleteBehavior.Cascade);

        //     var senhaHasheada = BC.HashPassword("Admin@123", 12);
        //     var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        //     modelBuilder.Entity<Usuario>().HasData(
        //         new
        //         {
        //             Id = adminId,
        //             Nome = "Administrador",
        //             Email = "admin@exemplo.com",
        //             Senha = senhaHasheada,
        //             DataCadastro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        //         },
        //         new
        //         {
        //             Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        //             Nome = "Usuário Teste",
        //             Email = "user@teste.com",
        //             Senha = BC.HashPassword("User@123", 12),
        //             DataCadastro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        //         }
        //     );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
