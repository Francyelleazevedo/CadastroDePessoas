using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Infra.Contexto;
using CadastroDePessoas.Infra.Repositorios;
using Microsoft.EntityFrameworkCore;

namespace CadastroDePessoas.Infra.Testes.Repositorios
{
    public class RepositorioPessoaTeste
    {
        private static DbContextOptions<AppDbContexto> ObterOpcoesBancoDados()
        {
            return new DbContextOptionsBuilder<AppDbContexto>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task AdicionarAsync_DevePersistirPessoa()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725"
            );

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new PessoaRepositorio(contexto);
                await repositorio.AdicionarAsync(pessoa);
            }

            // Assert
            using (var contexto = new AppDbContexto(options))
            {
                Assert.Equal(1, await contexto.Pessoas.CountAsync());
                var pessoaPersistida = await contexto.Pessoas.FirstOrDefaultAsync();
                Assert.NotNull(pessoaPersistida);
                Assert.Equal("João Silva", pessoaPersistida.Nome);
                Assert.Equal(Sexo.Masculino, pessoaPersistida.Sexo);
                Assert.Equal("joao@exemplo.com", pessoaPersistida.Email);
                Assert.Equal(new DateTime(1990, 1, 1), pessoaPersistida.DataNascimento);
                Assert.Equal("São Paulo", pessoaPersistida.Naturalidade);
                Assert.Equal("Brasileira", pessoaPersistida.Nacionalidade);
                Assert.Equal("52998224725", pessoaPersistida.Cpf);
            }
        }

        [Fact]
        public async Task CpfExisteAsync_ComCPFExistente_DeveRetornarTrue()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725"
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Pessoas.AddAsync(pessoa);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new PessoaRepositorio(contexto);
                var resultado = await repositorio.CpfExisteAsync("52998224725");

                // Assert
                Assert.True(resultado);
            }
        }

        [Fact]
        public async Task CpfExisteAsync_ComCPFInexistente_DeveRetornarFalse()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725"
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Pessoas.AddAsync(pessoa);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new PessoaRepositorio(contexto);
                var resultado = await repositorio.CpfExisteAsync("11144477735");

                // Assert
                Assert.False(resultado);
            }
        }

        [Fact]
        public async Task CpfExisteAsync_ComCPFExistenteEIgnorandoId_DeveRetornarFalse()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725"
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Pessoas.AddAsync(pessoa);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new PessoaRepositorio(contexto);
                var pessoaExistente = await contexto.Pessoas.FirstOrDefaultAsync();
                var resultado = await repositorio.CpfExisteAsync("52998224725", pessoaExistente.Id);

                // Assert
                Assert.False(resultado);
            }
        }

        [Fact]
        public async Task ObterPorIdAsync_ComIdExistente_DeveRetornarPessoa()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725"
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Pessoas.AddAsync(pessoa);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new PessoaRepositorio(contexto);
                var pessoaExistente = await contexto.Pessoas.FirstOrDefaultAsync();
                var resultado = await repositorio.ObterPorIdAsync(pessoaExistente.Id);

                // Assert
                Assert.NotNull(resultado);
                Assert.Equal(pessoaExistente.Id, resultado.Id);
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
                var repositorio = new PessoaRepositorio(contexto);
                var resultado = await repositorio.ObterPorIdAsync(Guid.NewGuid());

                // Assert
                Assert.Null(resultado);
            }
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarTodasPessoas()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa1 = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725"
            );

            var pessoa2 = new Domain.Entidades.Pessoa(
                "Maria Santos",
                Sexo.Feminino,
                "maria@exemplo.com",
                new DateTime(1992, 5, 10),
                "Rio de Janeiro",
                "Brasileira",
                "11144477735"
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Pessoas.AddRangeAsync(pessoa1, pessoa2);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new PessoaRepositorio(contexto);
                var resultado = await repositorio.ListarAsync();

                // Assert
                Assert.NotNull(resultado);
                Assert.Equal(2, resultado.Count());
            }
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarPessoa()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725"
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Pessoas.AddAsync(pessoa);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new PessoaRepositorio(contexto);
                var pessoaExistente = await contexto.Pessoas.FirstOrDefaultAsync();

                pessoaExistente.Atualizar(
                    "João Silva Atualizado",
                    Sexo.Masculino,
                    "joao.atualizado@exemplo.com",
                    new DateTime(1990, 1, 1),
                    "Rio de Janeiro",
                    "Brasileira"
                );

                await repositorio.AtualizarAsync(pessoaExistente);
            }

            // Assert
            using (var contexto = new AppDbContexto(options))
            {
                var pessoaAtualizada = await contexto.Pessoas.FirstOrDefaultAsync();
                Assert.NotNull(pessoaAtualizada);
                Assert.Equal("João Silva Atualizado", pessoaAtualizada.Nome);
                Assert.Equal("joao.atualizado@exemplo.com", pessoaAtualizada.Email);
                Assert.Equal("Rio de Janeiro", pessoaAtualizada.Naturalidade);
                Assert.NotNull(pessoaAtualizada.DataAtualizacao);
            }
        }

        [Fact]
        public async Task RemoverAsync_DeveRemoverPessoa()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725"
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Pessoas.AddAsync(pessoa);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new PessoaRepositorio(contexto);
                var pessoaExistente = await contexto.Pessoas.FirstOrDefaultAsync();
                await repositorio.RemoverAsync(pessoaExistente);
            }

            // Assert
            using (var contexto = new AppDbContexto(options))
            {
                Assert.Equal(0, await contexto.Pessoas.CountAsync());
            }
        }
    }
}
