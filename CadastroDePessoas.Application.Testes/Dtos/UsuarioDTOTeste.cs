using CadastroDePessoas.Application.Dtos.Usuario;
using Xunit;

namespace CadastroDePessoas.Application.Testes.Dtos
{
    public class UsuarioDTOTeste
    {
        [Fact]
        public void UsuarioDTO_PropriedadesDevemSerCorretamenteSetadas()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "João Silva";
            var email = "joao@email.com";
            var dataCadastro = new DateTime(2025, 1, 15);

            // Act
            var usuario = new UsuarioDTO
            {
                Id = id,
                Nome = nome,
                Email = email,
                DataCadastro = dataCadastro
            };

            // Assert
            Assert.Equal(id, usuario.Id);
            Assert.Equal(nome, usuario.Nome);
            Assert.Equal(email, usuario.Email);
            Assert.Equal(dataCadastro, usuario.DataCadastro);
        }

        [Fact]
        public void UsuarioDTO_ConstrutorPadrao_DeveCriarInstanciaValida()
        {
            // Act
            var usuario = new UsuarioDTO();

            // Assert
            Assert.NotNull(usuario);
            Assert.Equal(Guid.Empty, usuario.Id);
            Assert.Null(usuario.Nome);
            Assert.Null(usuario.Email);
            Assert.Equal(DateTime.MinValue, usuario.DataCadastro);
        }

        [Fact]
        public void UsuarioDTO_ConstrutorComParametros_DeveCriarInstanciaValida()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Maria Silva";
            var email = "maria@email.com";
            var dataCadastro = new DateTime(2025, 3, 10);

            // Act
            var usuario = new UsuarioDTO(id, nome, email, dataCadastro);

            // Assert
            Assert.Equal(id, usuario.Id);
            Assert.Equal(nome, usuario.Nome);
            Assert.Equal(email, usuario.Email);
            Assert.Equal(dataCadastro, usuario.DataCadastro);
        }

        [Fact]
        public void UsuarioDTO_TempoDesdeRegistro_DeveCalcularCorretamente()
        {
            // Arrange
            var dataCadastro = DateTime.Now.AddDays(-10); // 10 dias atrás
            var usuario = new UsuarioDTO
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                DataCadastro = dataCadastro
            };

            // Act
            var tempoDesdeRegistro = usuario.TempoDesdeRegistro;

            // Assert
            Assert.True(tempoDesdeRegistro.TotalDays >= 9.9 && tempoDesdeRegistro.TotalDays <= 10.1);
            Assert.True(tempoDesdeRegistro >= TimeSpan.Zero);
        }

        [Fact]
        public void UsuarioDTO_TempoDesdeRegistro_ComDataCadastroNoFuturo_DeveRetornarTimeSpanZero()
        {
            // Arrange
            var dataCadastroFutura = DateTime.Now.AddDays(5); // 5 dias no futuro
            var usuario = new UsuarioDTO
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                DataCadastro = dataCadastroFutura
            };

            // Act
            var tempoDesdeRegistro = usuario.TempoDesdeRegistro;

            // Assert
            Assert.Equal(TimeSpan.Zero, tempoDesdeRegistro);
        }

        [Fact]
        public void UsuarioDTO_EstaRegistradoHaMaisDeUmMes_DeveRetornarTrue_QuandoDataCadastroMaisDeUmMes()
        {
            // Arrange
            var dataCadastro = DateTime.Now.AddDays(-35); // 35 dias atrás (mais de um mês)
            var usuario = new UsuarioDTO
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                DataCadastro = dataCadastro
            };

            // Act & Assert
            Assert.True(usuario.EstaRegistradoHaMaisDeUmMes);
        }

        [Fact]
        public void UsuarioDTO_EstaRegistradoHaMaisDeUmMes_DeveRetornarFalse_QuandoDataCadastroMenosDeUmMes()
        {
            // Arrange
            var dataCadastro = DateTime.Now.AddDays(-20); // 20 dias atrás (menos de um mês)
            var usuario = new UsuarioDTO
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                DataCadastro = dataCadastro
            };

            // Act & Assert
            Assert.False(usuario.EstaRegistradoHaMaisDeUmMes);
        }

        [Fact]
        public void UsuarioDTO_ToString_DeveRetornarInfoFormatada()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "João Silva";
            var email = "joao@email.com";
            var dataCadastro = new DateTime(2025, 8, 27);

            var usuario = new UsuarioDTO(id, nome, email, dataCadastro);

            // Act
            var resultado = usuario.ToString();

            // Assert
            var esperado = $"Id: {id}, Nome: {nome}, Email: {email}, DataCadastro: 27/08/2025";
            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void UsuarioDTO_TempoDesdeRegistro_ComDataCadastroExatamenteAgora_DeveRetornarTimeSpanProximoDeZero()
        {
            // Arrange
            var dataCadastro = DateTime.Now;
            var usuario = new UsuarioDTO
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                DataCadastro = dataCadastro
            };

            // Act
            var tempoDesdeRegistro = usuario.TempoDesdeRegistro;

            // Assert
            Assert.True(tempoDesdeRegistro.TotalSeconds >= 0);
            Assert.True(tempoDesdeRegistro.TotalSeconds < 1); // Deve ser menos de 1 segundo
        }

        [Fact]
        public void UsuarioDTO_EstaRegistradoHaMaisDeUmMes_ComDataCadastroExatamente30Dias_DeveRetornarFalse()
        {
            // Arrange
            var dataCadastro = DateTime.Now.AddDays(-29); // 29 dias atrás para garantir menos de um mês
            var usuario = new UsuarioDTO
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                DataCadastro = dataCadastro
            };

            // Act & Assert
            Assert.False(usuario.EstaRegistradoHaMaisDeUmMes);
        }

        [Fact]
        public void UsuarioDTO_EstaRegistradoHaMaisDeUmMes_ComDataCadastro31Dias_DeveRetornarTrue()
        {
            // Arrange
            var dataCadastro = DateTime.Now.AddDays(-31); // 31 dias atrás (mais de 30)
            var usuario = new UsuarioDTO
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                DataCadastro = dataCadastro
            };

            // Act & Assert
            Assert.True(usuario.EstaRegistradoHaMaisDeUmMes);
        }
    }
}