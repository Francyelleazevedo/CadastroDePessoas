using CadastroDePessoas.Application.CQRS.Consultas.ObterPessoa;
using CadastroDePessoas.Application.Dtos.Pessoa;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Domain.Interfaces;
using Moq;
using Xunit;

namespace CadastroDePessoas.Application.Testes.Consultas
{
    public class ObterPessoaConsultaTeste
    {
        private readonly Mock<IPessoaRepositorio> _mockPessoaRepositorio;
        private readonly Mock<IServiceCache> _mockServiceCache;
        private readonly ObterPessoaConsultaHandler _handler;

        public ObterPessoaConsultaTeste()
        {
            _mockPessoaRepositorio = new Mock<IPessoaRepositorio>();
            _mockServiceCache = new Mock<IServiceCache>();
            _handler = new ObterPessoaConsultaHandler(_mockPessoaRepositorio.Object, _mockServiceCache.Object);
        }

        #region Testes da Consulta (DTO)

        [Fact]
        public void ObterPessoaConsulta_DeveImplementarIRequest()
        {
            // Arrange & Act
            var consulta = new ObterPessoaConsulta(Guid.NewGuid());

            // Assert
            Assert.IsAssignableFrom<MediatR.IRequest<PessoaDTO>>(consulta);
        }

        [Fact]
        public void ObterPessoaConsulta_ConstrutorComId_DeveDefinirIdCorretamente()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var consulta = new ObterPessoaConsulta(id);

            // Assert
            Assert.Equal(id, consulta.Id);
        }

        [Fact]
        public void ObterPessoaConsulta_Id_DeveSerSetavel()
        {
            // Arrange
            var consulta = new ObterPessoaConsulta(Guid.NewGuid());
            var novoId = Guid.NewGuid();

            // Act
            consulta.Id = novoId;

            // Assert
            Assert.Equal(novoId, consulta.Id);
        }

        [Fact]
        public void ObterPessoaConsulta_ComGuidEmpty_DevePermitir()
        {
            // Arrange & Act
            var consulta = new ObterPessoaConsulta(Guid.Empty);

            // Assert
            Assert.Equal(Guid.Empty, consulta.Id);
        }

        #endregion

        #region Testes do Handler - Construtor

        [Fact]
        public void Handler_ConstrutorComParametrosValidos_DeveInicializarCorretamente()
        {
            // Arrange & Act
            var handler = new ObterPessoaConsultaHandler(_mockPessoaRepositorio.Object, _mockServiceCache.Object);

            // Assert
            Assert.NotNull(handler);
        }

        [Fact]
        public void Handler_ConstrutorComRepositorioNulo_DeveLancarException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new ObterPessoaConsultaHandler(null!, _mockServiceCache.Object));
        }

        [Fact]
        public void Handler_ConstrutorComServiceCacheNulo_DeveLancarException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new ObterPessoaConsultaHandler(_mockPessoaRepositorio.Object, null!));
        }

        #endregion

        #region Testes do Handler - Cache Hit (Sucesso com Cache)

        [Fact]
        public async Task Handle_PessoaExisteNoCache_DeveRetornarDoCacheSemConsultarRepositorio()
        {
            // Arrange
            var id = Guid.NewGuid();
            var consulta = new ObterPessoaConsulta(id);
            var pessoaCache = new PessoaDTO
            {
                Id = id,
                Nome = "João Silva",
                Email = "joao@teste.com",
                Cpf = "12345678901"
            };

            _mockServiceCache.Setup(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"))
                           .ReturnsAsync(pessoaCache);

            // Act
            var resultado = await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(pessoaCache.Id, resultado.Id);
            Assert.Equal(pessoaCache.Nome, resultado.Nome);
            Assert.Equal(pessoaCache.Email, resultado.Email);
            
            _mockServiceCache.Verify(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"), Times.Once);
            _mockPessoaRepositorio.Verify(x => x.ObterPorIdAsync(It.IsAny<Guid>()), Times.Never);
            _mockServiceCache.Verify(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<PessoaDTO>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CacheRetornaNulo_DeveBuscarNoRepositorio()
        {
            // Arrange
            var id = Guid.NewGuid();
            var consulta = new ObterPessoaConsulta(id);
            var endereco = new Endereco("12345-678", "Rua Teste", "123", "Apto 1", "Centro", "Cidade", "SP");
            var pessoa = new Pessoa("João Silva", Sexo.Masculino, "joao@teste.com", DateTime.Now.AddYears(-30), "Cidade", "Brasil", "12345678901", endereco);

            _mockServiceCache.Setup(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"))
                           .ReturnsAsync((PessoaDTO)null!);

            _mockPessoaRepositorio.Setup(x => x.ObterPorIdAsync(id))
                                 .ReturnsAsync(pessoa);

            _mockServiceCache.Setup(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<PessoaDTO>(), It.IsAny<TimeSpan>()))
                           .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(pessoa.Nome, resultado.Nome);
            Assert.Equal(pessoa.Email, resultado.Email);
            
            _mockServiceCache.Verify(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"), Times.Once);
            _mockPessoaRepositorio.Verify(x => x.ObterPorIdAsync(id), Times.Once);
            _mockServiceCache.Verify(x => x.DefinirAsync($"pessoa_{id}", It.IsAny<PessoaDTO>(), TimeSpan.FromMinutes(5)), Times.Once);
        }

        #endregion

        #region Testes do Handler - Cache Miss e Repositório

        [Fact]
        public async Task Handle_PessoaExisteNoRepositorio_DeveRetornarPessoaEAdicionarAoCache()
        {
            // Arrange
            var id = Guid.NewGuid();
            var consulta = new ObterPessoaConsulta(id);
            var endereco = new Endereco("12345-678", "Rua Teste", "123", "Apto 1", "Centro", "Cidade", "SP");
            var pessoa = new Pessoa("Maria Silva", Sexo.Feminino, "maria@teste.com", DateTime.Now.AddYears(-25), "Cidade", "Brasil", "98765432100", endereco);

            _mockServiceCache.Setup(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"))
                           .ReturnsAsync((PessoaDTO)null!);

            _mockPessoaRepositorio.Setup(x => x.ObterPorIdAsync(id))
                                 .ReturnsAsync(pessoa);

            _mockServiceCache.Setup(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<PessoaDTO>(), It.IsAny<TimeSpan>()))
                           .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(pessoa.Nome, resultado.Nome);
            Assert.Equal(pessoa.Email, resultado.Email);
            Assert.Equal(pessoa.Cpf, resultado.Cpf);
            Assert.NotNull(resultado.Endereco);
            Assert.Equal(endereco.Cep, resultado.Endereco.Cep);
            
            _mockServiceCache.Verify(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"), Times.Once);
            _mockPessoaRepositorio.Verify(x => x.ObterPorIdAsync(id), Times.Once);
            _mockServiceCache.Verify(x => x.DefinirAsync($"pessoa_{id}", It.IsAny<PessoaDTO>(), TimeSpan.FromMinutes(5)), Times.Once);
        }

        [Fact]
        public async Task Handle_PessoaSemEndereco_DeveRetornarPessoaComEnderecoNulo()
        {
            // Arrange
            var id = Guid.NewGuid();
            var consulta = new ObterPessoaConsulta(id);
            var pessoa = new Pessoa("Pedro Silva", Sexo.Masculino, "pedro@teste.com", DateTime.Now.AddYears(-40), "Cidade", "Brasil", "11111111111");

            _mockServiceCache.Setup(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"))
                           .ReturnsAsync((PessoaDTO)null!);

            _mockPessoaRepositorio.Setup(x => x.ObterPorIdAsync(id))
                                 .ReturnsAsync(pessoa);

            _mockServiceCache.Setup(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<PessoaDTO>(), It.IsAny<TimeSpan>()))
                           .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(pessoa.Nome, resultado.Nome);
            Assert.Equal(pessoa.Email, resultado.Email);
            Assert.Null(resultado.Endereco);
            
            _mockServiceCache.Verify(x => x.DefinirAsync($"pessoa_{id}", It.IsAny<PessoaDTO>(), TimeSpan.FromMinutes(5)), Times.Once);
        }

        [Fact]
        public async Task Handle_PessoaNaoEncontrada_DeveLancarException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var consulta = new ObterPessoaConsulta(id);

            _mockServiceCache.Setup(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"))
                           .ReturnsAsync((PessoaDTO)null!);

            _mockPessoaRepositorio.Setup(x => x.ObterPorIdAsync(id))
                                 .ReturnsAsync((Pessoa)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(consulta, CancellationToken.None));

            Assert.Equal("Pessoa não encontrada", exception.Message);
            
            _mockServiceCache.Verify(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"), Times.Once);
            _mockPessoaRepositorio.Verify(x => x.ObterPorIdAsync(id), Times.Once);
            _mockServiceCache.Verify(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<PessoaDTO>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        #endregion

        #region Testes de Cache e Performance

        [Fact]
        public async Task Handle_CacheKeyCorreto_DeveUsarChaveCorreta()
        {
            // Arrange
            var id = Guid.NewGuid();
            var consulta = new ObterPessoaConsulta(id);
            var expectedCacheKey = $"pessoa_{id}";

            _mockServiceCache.Setup(x => x.ObterAsync<PessoaDTO>(expectedCacheKey))
                           .ReturnsAsync((PessoaDTO)null!);

            var endereco = new Endereco("12345-678", "Rua Teste", "123", "Apto 1", "Centro", "Cidade", "SP");
            var pessoa = new Pessoa("Teste", Sexo.Masculino, "teste@teste.com", DateTime.Now.AddYears(-30), "Cidade", "Brasil", "12345678901", endereco);

            _mockPessoaRepositorio.Setup(x => x.ObterPorIdAsync(id))
                                 .ReturnsAsync(pessoa);

            // Act
            await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            _mockServiceCache.Verify(x => x.ObterAsync<PessoaDTO>(expectedCacheKey), Times.Once);
            _mockServiceCache.Verify(x => x.DefinirAsync(expectedCacheKey, It.IsAny<PessoaDTO>(), TimeSpan.FromMinutes(5)), Times.Once);
        }

        [Fact]
        public async Task Handle_TempoExpiracao_DeveUsar5Minutos()
        {
            // Arrange
            var id = Guid.NewGuid();
            var consulta = new ObterPessoaConsulta(id);
            var expectedExpiration = TimeSpan.FromMinutes(5);

            _mockServiceCache.Setup(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"))
                           .ReturnsAsync((PessoaDTO)null!);

            var pessoa = new Pessoa("Teste", Sexo.Masculino, "teste@teste.com", DateTime.Now.AddYears(-30), "Cidade", "Brasil", "12345678901");

            _mockPessoaRepositorio.Setup(x => x.ObterPorIdAsync(id))
                                 .ReturnsAsync(pessoa);

            // Act
            await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            _mockServiceCache.Verify(x => x.DefinirAsync($"pessoa_{id}", It.IsAny<PessoaDTO>(), expectedExpiration), Times.Once);
        }

        #endregion

        #region Testes de Cancellation Token

        [Fact]
        public async Task Handle_CancellationTokenCancelado_DeveRespeitarCancelamento()
        {
            // Arrange
            var id = Guid.NewGuid();
            var consulta = new ObterPessoaConsulta(id);
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _mockServiceCache.Setup(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"))
                           .ReturnsAsync((PessoaDTO)null!);

            _mockPessoaRepositorio.Setup(x => x.ObterPorIdAsync(id))
                                 .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => _handler.Handle(consulta, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task Handle_CancellationTokenValido_DeveExecutarNormalmente()
        {
            // Arrange
            var id = Guid.NewGuid();
            var consulta = new ObterPessoaConsulta(id);
            var cancellationTokenSource = new CancellationTokenSource();

            var pessoaCache = new PessoaDTO
            {
                Id = id,
                Nome = "Teste Cache",
                Email = "cache@teste.com",
                Cpf = "12345678901"
            };

            _mockServiceCache.Setup(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"))
                           .ReturnsAsync(pessoaCache);

            // Act
            var resultado = await _handler.Handle(consulta, cancellationTokenSource.Token);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(pessoaCache.Nome, resultado.Nome);
        }

        #endregion

        #region Testes de Edge Cases

        [Fact]
        public async Task Handle_GuidEmpty_DeveBuscarComIdEmpty()
        {
            // Arrange
            var consulta = new ObterPessoaConsulta(Guid.Empty);

            _mockServiceCache.Setup(x => x.ObterAsync<PessoaDTO>($"pessoa_{Guid.Empty}"))
                           .ReturnsAsync((PessoaDTO)null!);

            _mockPessoaRepositorio.Setup(x => x.ObterPorIdAsync(Guid.Empty))
                                 .ReturnsAsync((Pessoa)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(consulta, CancellationToken.None));

            Assert.Equal("Pessoa não encontrada", exception.Message);
            _mockPessoaRepositorio.Verify(x => x.ObterPorIdAsync(Guid.Empty), Times.Once);
        }

        [Fact]
        public async Task Handle_CacheComException_DeveContinuarComRepositorio()
        {
            // Arrange
            var id = Guid.NewGuid();
            var consulta = new ObterPessoaConsulta(id);

            _mockServiceCache.Setup(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"))
                           .ThrowsAsync(new Exception("Erro no cache"));

            var pessoa = new Pessoa("Teste", Sexo.Masculino, "teste@teste.com", DateTime.Now.AddYears(-30), "Cidade", "Brasil", "12345678901");

            _mockPessoaRepositorio.Setup(x => x.ObterPorIdAsync(id))
                                 .ReturnsAsync(pessoa);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(consulta, CancellationToken.None));

            Assert.Equal("Erro no cache", exception.Message);
        }

        [Fact]
        public async Task Handle_RepositorioComException_DevePropagar()
        {
            // Arrange
            var id = Guid.NewGuid();
            var consulta = new ObterPessoaConsulta(id);

            _mockServiceCache.Setup(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"))
                           .ReturnsAsync((PessoaDTO)null!);

            _mockPessoaRepositorio.Setup(x => x.ObterPorIdAsync(id))
                                 .ThrowsAsync(new Exception("Erro no repositório"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(consulta, CancellationToken.None));

            Assert.Equal("Erro no repositório", exception.Message);
        }

        #endregion

        #region Testes de Integração entre Cache e Repositório

        [Fact]
        public async Task Handle_FluxoCompleto_CacheMissEDepoisCacheHit()
        {
            // Arrange
            var id = Guid.NewGuid();
            var consulta = new ObterPessoaConsulta(id);
            var pessoa = new Pessoa("Teste Integração", Sexo.Masculino, "integracao@teste.com", DateTime.Now.AddYears(-30), "Cidade", "Brasil", "12345678901");

            // Primeira chamada: cache miss
            _mockServiceCache.SetupSequence(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"))
                           .ReturnsAsync((PessoaDTO)null!) // Cache miss na primeira chamada
                           .ReturnsAsync(new PessoaDTO // Cache hit na segunda chamada
                           {
                               Id = pessoa.Id,
                               Nome = pessoa.Nome,
                               Email = pessoa.Email,
                               Cpf = pessoa.Cpf
                           });

            _mockPessoaRepositorio.Setup(x => x.ObterPorIdAsync(id))
                                 .ReturnsAsync(pessoa);

            _mockServiceCache.Setup(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<PessoaDTO>(), It.IsAny<TimeSpan>()))
                           .Returns(Task.CompletedTask);

            // Act - Primeira chamada (cache miss)
            var resultado1 = await _handler.Handle(consulta, CancellationToken.None);

            // Act - Segunda chamada (cache hit)
            var resultado2 = await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado1);
            Assert.NotNull(resultado2);
            Assert.Equal(pessoa.Nome, resultado1.Nome);
            Assert.Equal(pessoa.Nome, resultado2.Nome);

            // Repositório deve ser chamado apenas uma vez (cache miss)
            _mockPessoaRepositorio.Verify(x => x.ObterPorIdAsync(id), Times.Once);
            
            // Cache deve ser consultado duas vezes
            _mockServiceCache.Verify(x => x.ObterAsync<PessoaDTO>($"pessoa_{id}"), Times.Exactly(2));
            
            // Cache deve ser definido apenas uma vez (após cache miss)
            _mockServiceCache.Verify(x => x.DefinirAsync($"pessoa_{id}", It.IsAny<PessoaDTO>(), TimeSpan.FromMinutes(5)), Times.Once);
        }

        #endregion
    }
}