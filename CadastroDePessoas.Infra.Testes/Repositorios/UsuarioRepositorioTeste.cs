using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Infra.Contexto;
using CadastroDePessoas.Infra.Repositorios;
using Microsoft.EntityFrameworkCore;

namespace CadastroDePessoas.Infra.Testes.Repositorios
{
    public class UsuarioRepositorioTeste
    {
        private static DbContextOptions<AppDbContexto> ObterOpcoesBancoDados()
        {
            return new DbContextOptionsBuilder<AppDbContexto>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task AdicionarAsync_DevePersistirUsuario()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var usuario = new Usuario(
                "João Silva",
                "joao@exemplo.com",
                "senha123"
            );

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new UsuarioRepositorio(contexto);
                await repositorio.AdicionarAsync(usuario);
            }

            // Assert
            using (var contexto = new AppDbContexto(options))
            {
                Assert.Equal(1, await contexto.Usuarios.CountAsync());
                var usuarioPersistido = await contexto.Usuarios.FirstOrDefaultAsync();
                Assert.NotNull(usuarioPersistido);
                Assert.Equal("João Silva", usuarioPersistido.Nome);
                Assert.Equal("joao@exemplo.com", usuarioPersistido.Email);
                Assert.Equal("senha123", usuarioPersistido.Senha);
            }
        }

        [Fact]
        public async Task ObterPorEmailAsync_ComEmailExistente_DeveRetornarUsuario()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var usuario = new Usuario(
                "João Silva",
                "joao@exemplo.com",
                "senha123"
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Usuarios.AddAsync(usuario);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new UsuarioRepositorio(contexto);
                var resultado = await repositorio.ObterPorEmailAsync("joao@exemplo.com");

                // Assert
                Assert.NotNull(resultado);
                Assert.Equal("João Silva", resultado.Nome);
                Assert.Equal("joao@exemplo.com", resultado.Email);
            }
        }

        [Fact]
        public async Task ObterPorEmailAsync_ComEmailInexistente_DeveRetornarNull()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var usuario = new Usuario(
                "João Silva",
                "joao@exemplo.com",
                "senha123"
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Usuarios.AddAsync(usuario);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new UsuarioRepositorio(contexto);
                var resultado = await repositorio.ObterPorEmailAsync("maria@exemplo.com");

                // Assert
                Assert.Null(resultado);
            }
        }

        [Fact]
        public async Task ObterPorIdAsync_ComIdExistente_DeveRetornarUsuario()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var usuario = new Usuario(
                "João Silva",
                "joao@exemplo.com",
                "senha123"
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Usuarios.AddAsync(usuario);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new UsuarioRepositorio(contexto);
                var usuarioExistente = await contexto.Usuarios.FirstOrDefaultAsync();
                var resultado = await repositorio.ObterPorIdAsync(usuarioExistente.Id);

                // Assert
                Assert.NotNull(resultado);
                Assert.Equal(usuarioExistente.Id, resultado.Id);
                Assert.Equal("João Silva", resultado.Nome);
            }
        }

        [Fact]
        public async Task ObterPorIdAsync_ComIdInexistente_DeveRetornarNull()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new UsuarioRepositorio(contexto);
                var resultado = await repositorio.ObterPorIdAsync(Guid.NewGuid());

                // Assert
                Assert.Null(resultado);
            }
        }

        [Fact]
        public async Task ListarAsync_DeveRetornarTodosUsuarios()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var usuario1 = new Usuario(
                "João Silva",
                "joao@exemplo.com",
                "senha123"
            );

            var usuario2 = new Usuario(
                "Maria Santos",
                "maria@exemplo.com",
                "senha456"
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Usuarios.AddRangeAsync(usuario1, usuario2);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new UsuarioRepositorio(contexto);
                var resultado = await repositorio.ListarAsync();

                // Assert
                Assert.NotNull(resultado);
                Assert.Equal(2, resultado.Count());
            }
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarUsuario()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var usuario = new Usuario(
                "João Silva",
                "joao@exemplo.com",
                "senha123"
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Usuarios.AddAsync(usuario);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new UsuarioRepositorio(contexto);
                var usuarioExistente = await contexto.Usuarios.FirstOrDefaultAsync();

                usuarioExistente.AlterarSenha("novaSenha789");
                await repositorio.AtualizarAsync(usuarioExistente);
            }

            // Assert
            using (var contexto = new AppDbContexto(options))
            {
                var usuarioAtualizado = await contexto.Usuarios.FirstOrDefaultAsync();
                Assert.NotNull(usuarioAtualizado);
                Assert.Equal("novaSenha789", usuarioAtualizado.Senha);
            }
        }

        [Fact]
        public async Task RemoverAsync_DeveRemoverUsuario()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var usuario = new Usuario(
                "João Silva",
                "joao@exemplo.com",
                "senha123"
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Usuarios.AddAsync(usuario);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new UsuarioRepositorio(contexto);
                var usuarioExistente = await contexto.Usuarios.FirstOrDefaultAsync();
                await repositorio.RemoverAsync(usuarioExistente);
            }

            // Assert
            using (var contexto = new AppDbContexto(options))
            {
                Assert.Equal(0, await contexto.Usuarios.CountAsync());
            }
        }
    }
}
