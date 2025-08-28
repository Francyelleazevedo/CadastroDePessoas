using CadastroDePessoas.Application.Services;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text;
using System.Text.Json;
using Xunit;

namespace CadastroDePessoas.Application.Testes.Services
{
    public class ServiceCacheTeste
    {
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly ServicoCache _servicoCache;

        public ServiceCacheTeste()
        {
            _mockCache = new Mock<IDistributedCache>();
            _servicoCache = new ServicoCache(_mockCache.Object);
        }

        #region Testes do Construtor

        [Fact]
        public void Construtor_QuandoChamado_DeveInicializarCorretamente()
        {
            // Arrange & Act
            var servico = new ServicoCache(_mockCache.Object);

            // Assert
            Assert.NotNull(servico);
        }

        #endregion

        #region Testes ObterAsync

        [Fact]
        public async Task ObterAsync_ComChaveValida_DeveRetornarObjetoDeserializado()
        {
            // Arrange
            var chave = "teste_chave";
            var objetoTeste = new { Nome = "João", Idade = 30 };
            var json = JsonSerializer.Serialize(objetoTeste);
            var bytes = Encoding.UTF8.GetBytes(json);

            _mockCache.Setup(x => x.GetAsync(chave, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(bytes);

            // Act
            var resultado = await _servicoCache.ObterAsync<dynamic>(chave);

            // Assert
            Assert.NotNull(resultado);
            _mockCache.Verify(x => x.GetAsync(chave, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ObterAsync_ComChaveNula_DeveLancarArgumentException()
        {
            // Arrange
            string chave = null!;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _servicoCache.ObterAsync<string>(chave));

            Assert.Equal("A chave não pode ser nula ou vazia (Parameter 'chave')", exception.Message);
            Assert.Equal("chave", exception.ParamName);
        }

        [Fact]
        public async Task ObterAsync_ComChaveVazia_DeveLancarArgumentException()
        {
            // Arrange
            var chave = "";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _servicoCache.ObterAsync<string>(chave));

            Assert.Equal("A chave não pode ser nula ou vazia (Parameter 'chave')", exception.Message);
            Assert.Equal("chave", exception.ParamName);
        }

        [Fact]
        public async Task ObterAsync_ComChaveEspacosEmBranco_DeveRetornarDefault()
        {
            // Arrange
            var chave = "   ";
            _mockCache.Setup(x => x.GetAsync(chave, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((byte[])null);

            // Act
            var resultado = await _servicoCache.ObterAsync<string>(chave);

            // Assert
            Assert.Null(resultado);
            _mockCache.Verify(x => x.GetAsync(chave, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ObterAsync_QuandoCacheRetornaNulo_DeveRetornarDefault()
        {
            // Arrange
            var chave = "chave_inexistente";
            _mockCache.Setup(x => x.GetAsync(chave, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((byte[])null);

            // Act
            var resultado = await _servicoCache.ObterAsync<string>(chave);

            // Assert
            Assert.Null(resultado);
            _mockCache.Verify(x => x.GetAsync(chave, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ObterAsync_QuandoCacheRetornaArrayVazio_DeveRetornarDefault()
        {
            // Arrange
            var chave = "chave_array_vazio";
            _mockCache.Setup(x => x.GetAsync(chave, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new byte[0]);

            // Act
            var resultado = await _servicoCache.ObterAsync<string>(chave);

            // Assert
            Assert.Null(resultado);
            _mockCache.Verify(x => x.GetAsync(chave, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ObterAsync_ComJsonInvalido_DeveRetornarDefault()
        {
            // Arrange
            var chave = "chave_json_invalido";
            var bytesInvalidos = Encoding.UTF8.GetBytes("{ json inválido }");

            _mockCache.Setup(x => x.GetAsync(chave, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(bytesInvalidos);

            // Act
            var resultado = await _servicoCache.ObterAsync<object>(chave);

            // Assert
            Assert.Null(resultado);
            _mockCache.Verify(x => x.GetAsync(chave, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("string_simples", "Texto de teste")]
        [InlineData("numero_inteiro", 42)]
        [InlineData("numero_decimal", 3.14)]
        [InlineData("boolean_true", true)]
        [InlineData("boolean_false", false)]
        public async Task ObterAsync_ComTiposPrimitivos_DeveDeserializarCorretamente<T>(string chave, T valor)
        {
            // Arrange
            var json = JsonSerializer.Serialize(valor);
            var bytes = Encoding.UTF8.GetBytes(json);

            _mockCache.Setup(x => x.GetAsync(chave, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(bytes);

            // Act
            var resultado = await _servicoCache.ObterAsync<T>(chave);

            // Assert
            Assert.Equal(valor, resultado);
            _mockCache.Verify(x => x.GetAsync(chave, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ObterAsync_ComObjetoComplexo_DeveDeserializarCorretamente()
        {
            // Arrange
            var chave = "objeto_complexo";
            var objeto = new TestObject 
            { 
                Id = 1, 
                Nome = "Teste", 
                Ativo = true,
                DataCriacao = DateTime.Now.Date,
                Lista = new List<string> { "item1", "item2" }
            };

            var json = JsonSerializer.Serialize(objeto);
            var bytes = Encoding.UTF8.GetBytes(json);

            _mockCache.Setup(x => x.GetAsync(chave, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(bytes);

            // Act
            var resultado = await _servicoCache.ObterAsync<TestObject>(chave);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(objeto.Id, resultado.Id);
            Assert.Equal(objeto.Nome, resultado.Nome);
            Assert.Equal(objeto.Ativo, resultado.Ativo);
            Assert.Equal(objeto.DataCriacao, resultado.DataCriacao);
            Assert.Equal(objeto.Lista, resultado.Lista);
        }

        #endregion

        #region Testes DefinirAsync

        [Fact]
        public async Task DefinirAsync_ComParametrosValidos_DeveSerializarEArmazenar()
        {
            // Arrange
            var chave = "teste_definir";
            var valor = "Valor de teste";
            var tempoExpiracao = TimeSpan.FromMinutes(5);

            // Act
            await _servicoCache.DefinirAsync(chave, valor, tempoExpiracao);

            // Assert
            _mockCache.Verify(x => x.SetAsync(
                chave,
                It.Is<byte[]>(b => Encoding.UTF8.GetString(b).Contains(valor)),
                It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == tempoExpiracao),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task DefinirAsync_SemTempoExpiracao_DeveUsarTempoDefault()
        {
            // Arrange
            var chave = "teste_default";
            var valor = "Valor teste";

            // Act
            await _servicoCache.DefinirAsync(chave, valor);

            // Assert
            _mockCache.Verify(x => x.SetAsync(
                chave,
                It.IsAny<byte[]>(),
                It.Is<DistributedCacheEntryOptions>(o => 
                    o.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(10)),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task DefinirAsync_ComChaveNula_DeveLancarArgumentException()
        {
            // Arrange
            string chave = null!;
            var valor = "teste";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _servicoCache.DefinirAsync(chave, valor));

            Assert.Equal("A chave não pode ser nula ou vazia (Parameter 'chave')", exception.Message);
            Assert.Equal("chave", exception.ParamName);
        }

        [Fact]
        public async Task DefinirAsync_ComChaveVazia_DeveLancarArgumentException()
        {
            // Arrange
            var chave = "";
            var valor = "teste";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _servicoCache.DefinirAsync(chave, valor));

            Assert.Equal("A chave não pode ser nula ou vazia (Parameter 'chave')", exception.Message);
            Assert.Equal("chave", exception.ParamName);
        }

        [Fact]
        public async Task DefinirAsync_ComValorNulo_DeveLancarArgumentNullException()
        {
            // Arrange
            var chave = "chave_teste";
            object valor = null!;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _servicoCache.DefinirAsync(chave, valor));

            Assert.Equal("O valor não pode ser nulo (Parameter 'valor')", exception.Message);
            Assert.Equal("valor", exception.ParamName);
        }

        [Fact]
        public async Task DefinirAsync_ComObjetoComplexo_DeveSerializarCorretamente()
        {
            // Arrange
            var chave = "objeto_teste";
            var objeto = new TestObject 
            { 
                Id = 123, 
                Nome = "Objeto Teste",
                Ativo = true,
                DataCriacao = DateTime.Now.Date,
                Lista = new List<string> { "a", "b", "c" }
            };

            // Act
            await _servicoCache.DefinirAsync(chave, objeto);

            // Assert
            _mockCache.Verify(x => x.SetAsync(
                chave,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(30)]
        [InlineData(60)]
        public async Task DefinirAsync_ComDiferentesTemposExpiracao_DeveConfigurarCorretamente(int minutos)
        {
            // Arrange
            var chave = $"teste_{minutos}min";
            var valor = "teste";
            var tempo = TimeSpan.FromMinutes(minutos);

            // Act
            await _servicoCache.DefinirAsync(chave, valor, tempo);

            // Assert
            _mockCache.Verify(x => x.SetAsync(
                chave,
                It.IsAny<byte[]>(),
                It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == tempo),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        #endregion

        #region Testes RemoverAsync

        [Fact]
        public async Task RemoverAsync_ComChaveValida_DeveChamarRemoveCache()
        {
            // Arrange
            var chave = "chave_remover";

            // Act
            await _servicoCache.RemoverAsync(chave);

            // Assert
            _mockCache.Verify(x => x.RemoveAsync(chave, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_ComChaveNula_DeveLancarArgumentException()
        {
            // Arrange
            string chave = null!;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _servicoCache.RemoverAsync(chave));

            Assert.Equal("A chave não pode ser nula ou vazia (Parameter 'chave')", exception.Message);
            Assert.Equal("chave", exception.ParamName);
        }

        [Fact]
        public async Task RemoverAsync_ComChaveVazia_DeveLancarArgumentException()
        {
            // Arrange
            var chave = "";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _servicoCache.RemoverAsync(chave));

            Assert.Equal("A chave não pode ser nula ou vazia (Parameter 'chave')", exception.Message);
            Assert.Equal("chave", exception.ParamName);
        }

        [Fact]
        public async Task RemoverAsync_ComChaveEspacosEmBranco_DeveChamarRemoveCache()
        {
            // Arrange
            var chave = "   ";

            // Act
            await _servicoCache.RemoverAsync(chave);

            // Assert
            _mockCache.Verify(x => x.RemoveAsync(chave, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("chave_simples")]
        [InlineData("chave:com:dois:pontos")]
        [InlineData("chave_com_underline")]
        [InlineData("chave-com-hifen")]
        [InlineData("chave.com.pontos")]
        public async Task RemoverAsync_ComDiferentesFormatosChave_DeveFuncionar(string chave)
        {
            // Act
            await _servicoCache.RemoverAsync(chave);

            // Assert
            _mockCache.Verify(x => x.RemoveAsync(chave, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Testes de Integração e Cenários Complexos

        [Fact]
        public async Task FluxoCompleto_DefinirObterRemover_DeveFuncionarCorretamente()
        {
            // Arrange
            var chave = "fluxo_completo";
            var valor = new TestObject { Id = 999, Nome = "Fluxo Teste" };
            var json = JsonSerializer.Serialize(valor);
            var bytes = Encoding.UTF8.GetBytes(json);

            _mockCache.Setup(x => x.GetAsync(chave, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(bytes);

            // Act & Assert - Definir
            await _servicoCache.DefinirAsync(chave, valor);
            _mockCache.Verify(x => x.SetAsync(
                chave, 
                It.IsAny<byte[]>(), 
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);

            // Act & Assert - Obter
            var resultado = await _servicoCache.ObterAsync<TestObject>(chave);
            Assert.NotNull(resultado);
            Assert.Equal(valor.Id, resultado.Id);
            Assert.Equal(valor.Nome, resultado.Nome);

            // Act & Assert - Remover
            await _servicoCache.RemoverAsync(chave);
            _mockCache.Verify(x => x.RemoveAsync(chave, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SerializacaoComPropriedadesCaseInsensitive_DeveFuncionar()
        {
            // Arrange
            var chave = "case_insensitive";
            var jsonComCaseDiferente = """{"id": 1, "NOME": "Teste", "ativo": true}""";
            var bytes = Encoding.UTF8.GetBytes(jsonComCaseDiferente);

            _mockCache.Setup(x => x.GetAsync(chave, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(bytes);

            // Act
            var resultado = await _servicoCache.ObterAsync<TestObject>(chave);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Teste", resultado.Nome);
            Assert.True(resultado.Ativo);
        }

        #endregion

        #region Classe Helper para Testes

        public class TestObject
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public bool Ativo { get; set; }
            public DateTime DataCriacao { get; set; }
            public List<string> Lista { get; set; } = new();
        }

        #endregion
    }
}
