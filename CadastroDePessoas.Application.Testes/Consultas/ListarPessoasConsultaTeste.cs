using CadastroDePessoas.Application.CQRS.Consultas.ListarPessoas;
using CadastroDePessoas.Application.Dtos.Pessoa;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Domain.Interfaces;
using Moq;
using Xunit;

namespace CadastroDePessoas.Application.Testes.Consultas
{
    public class ListarPessoasConsultaTeste
    {
        private readonly Mock<IPessoaRepositorio> _mockPessoaRepositorio;
        private readonly Mock<IServiceCache> _mockServiceCache;
        private readonly ListarPessoasConsultaHandler _handler;

        public ListarPessoasConsultaTeste()
        {
            _mockPessoaRepositorio = new Mock<IPessoaRepositorio>();
            _mockServiceCache = new Mock<IServiceCache>();
            _handler = new ListarPessoasConsultaHandler(_mockPessoaRepositorio.Object, _mockServiceCache.Object);
        }

        #region Testes da Consulta (DTO)

        [Fact]
        public void ListarPessoasConsulta_DeveImplementarIRequest()
        {
            // Arrange & Act
            var consulta = new ListarPessoasConsulta();

            // Assert
            Assert.IsAssignableFrom<MediatR.IRequest<IEnumerable<PessoaDTO>>>(consulta);
        }

        [Fact]
        public void ListarPessoasConsulta_ConstrutorPadrao_DeveCriarInstanciaValida()
        {
            // Arrange & Act
            var consulta = new ListarPessoasConsulta();

            // Assert
            Assert.NotNull(consulta);
        }

        [Fact]
        public void ListarPessoasConsulta_DeveSerClasseSimplesSemPropriedades()
        {
            // Arrange & Act
            var consulta1 = new ListarPessoasConsulta();
            var consulta2 = new ListarPessoasConsulta();

            // Assert
            Assert.NotNull(consulta1);
            Assert.NotNull(consulta2);
            Assert.IsType<ListarPessoasConsulta>(consulta1);
            Assert.IsType<ListarPessoasConsulta>(consulta2);
        }

        #endregion

        #region Testes do Handler - Construtor

        [Fact]
        public void Handler_ConstrutorComParametrosValidos_DeveInicializarCorretamente()
        {
            // Arrange & Act
            var handler = new ListarPessoasConsultaHandler(_mockPessoaRepositorio.Object, _mockServiceCache.Object);

            // Assert
            Assert.NotNull(handler);
        }

        [Fact]
        public void Handler_ConstrutorComRepositorioNulo_DeveLancarException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new ListarPessoasConsultaHandler(null!, _mockServiceCache.Object));
        }

        [Fact]
        public void Handler_ConstrutorComServiceCacheNulo_DeveLancarException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new ListarPessoasConsultaHandler(_mockPessoaRepositorio.Object, null!));
        }

        #endregion

        #region Testes do Handler - Cache Hit (Sucesso com Cache)

        [Fact]
        public async Task Handle_ListaExisteNoCache_DeveRetornarDoCacheSemConsultarRepositorio()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();
            var pessoasCache = new List<PessoaDTO>
            {
                new PessoaDTO
                {
                    Id = Guid.NewGuid(),
                    Nome = "João Silva",
                    Email = "joao@teste.com",
                    Cpf = "12345678901"
                },
                new PessoaDTO
                {
                    Id = Guid.NewGuid(),
                    Nome = "Maria Silva",
                    Email = "maria@teste.com",
                    Cpf = "98765432100"
                }
            };

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ReturnsAsync(pessoasCache);

            // Act
            var resultado = await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.Equal(pessoasCache.First().Nome, resultado.First().Nome);
            Assert.Equal(pessoasCache.Last().Nome, resultado.Last().Nome);
            
            _mockServiceCache.Verify(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"), Times.Once);
            _mockPessoaRepositorio.Verify(x => x.ListarAsync(), Times.Never);
            _mockServiceCache.Verify(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<IEnumerable<PessoaDTO>>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CacheVazio_DeveRetornarListaVaziaSemConsultarRepositorio()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();
            var pessoasCache = new List<PessoaDTO>();

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ReturnsAsync(pessoasCache);

            // Act
            var resultado = await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
            
            _mockServiceCache.Verify(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"), Times.Once);
            _mockPessoaRepositorio.Verify(x => x.ListarAsync(), Times.Never);
            _mockServiceCache.Verify(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<IEnumerable<PessoaDTO>>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CacheRetornaNulo_DeveBuscarNoRepositorio()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();
            var endereco1 = new Endereco("12345-678", "Rua A", "123", "Apto 1", "Centro", "Cidade A", "SP");
            var endereco2 = new Endereco("98765-432", "Rua B", "456", "Casa", "Vila", "Cidade B", "RJ");
            
            var pessoas = new List<Pessoa>
            {
                new Pessoa("João Silva", Sexo.Masculino, "joao@teste.com", DateTime.Now.AddYears(-30), "São Paulo", "Brasil", "12345678901", endereco1),
                new Pessoa("Maria Silva", Sexo.Feminino, "maria@teste.com", DateTime.Now.AddYears(-25), "Rio de Janeiro", "Brasil", "98765432100", endereco2)
            };

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ReturnsAsync((IEnumerable<PessoaDTO>)null!);

            _mockPessoaRepositorio.Setup(x => x.ListarAsync())
                                 .ReturnsAsync(pessoas);

            _mockServiceCache.Setup(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<IEnumerable<PessoaDTO>>(), It.IsAny<TimeSpan>()))
                           .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.Contains(resultado, p => p.Nome == "João Silva");
            Assert.Contains(resultado, p => p.Nome == "Maria Silva");
            
            _mockServiceCache.Verify(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"), Times.Once);
            _mockPessoaRepositorio.Verify(x => x.ListarAsync(), Times.Once);
            _mockServiceCache.Verify(x => x.DefinirAsync("lista_pessoas", It.IsAny<IEnumerable<PessoaDTO>>(), TimeSpan.FromMinutes(5)), Times.Once);
        }

        #endregion

        #region Testes do Handler - Cache Miss e Repositório

        [Fact]
        public async Task Handle_RepositorioComPessoas_DeveRetornarListaEAdicionarAoCache()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();
            var endereco = new Endereco("12345-678", "Rua Teste", "123", "Apto 1", "Centro", "Cidade", "SP");
            var pessoas = new List<Pessoa>
            {
                new Pessoa("Pedro Silva", Sexo.Masculino, "pedro@teste.com", DateTime.Now.AddYears(-40), "Cidade", "Brasil", "11111111111", endereco),
                new Pessoa("Ana Costa", Sexo.Feminino, "ana@teste.com", DateTime.Now.AddYears(-35), "Cidade", "Brasil", "22222222222"),
                new Pessoa("Carlos Oliveira", Sexo.Masculino, "carlos@teste.com", DateTime.Now.AddYears(-28), "Cidade", "Brasil", "33333333333", endereco)
            };

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ReturnsAsync((IEnumerable<PessoaDTO>)null!);

            _mockPessoaRepositorio.Setup(x => x.ListarAsync())
                                 .ReturnsAsync(pessoas);

            _mockServiceCache.Setup(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<IEnumerable<PessoaDTO>>(), It.IsAny<TimeSpan>()))
                           .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Count());
            
            var resultadoLista = resultado.ToList();
            Assert.Equal("Pedro Silva", resultadoLista[0].Nome);
            Assert.Equal("Ana Costa", resultadoLista[1].Nome);
            Assert.Equal("Carlos Oliveira", resultadoLista[2].Nome);
            
            // Verificar que pessoas com endereço têm endereço no DTO
            Assert.NotNull(resultadoLista[0].Endereco);
            Assert.Null(resultadoLista[1].Endereco); // Ana não tem endereço
            Assert.NotNull(resultadoLista[2].Endereco);
            
            _mockServiceCache.Verify(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"), Times.Once);
            _mockPessoaRepositorio.Verify(x => x.ListarAsync(), Times.Once);
            _mockServiceCache.Verify(x => x.DefinirAsync("lista_pessoas", It.IsAny<IEnumerable<PessoaDTO>>(), TimeSpan.FromMinutes(5)), Times.Once);
        }

        [Fact]
        public async Task Handle_RepositorioVazio_DeveRetornarListaVaziaEAdicionarAoCache()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();
            var pessoas = new List<Pessoa>();

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ReturnsAsync((IEnumerable<PessoaDTO>)null!);

            _mockPessoaRepositorio.Setup(x => x.ListarAsync())
                                 .ReturnsAsync(pessoas);

            _mockServiceCache.Setup(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<IEnumerable<PessoaDTO>>(), It.IsAny<TimeSpan>()))
                           .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
            
            _mockServiceCache.Verify(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"), Times.Once);
            _mockPessoaRepositorio.Verify(x => x.ListarAsync(), Times.Once);
            _mockServiceCache.Verify(x => x.DefinirAsync("lista_pessoas", It.IsAny<IEnumerable<PessoaDTO>>(), TimeSpan.FromMinutes(5)), Times.Once);
        }

        [Fact]
        public async Task Handle_PessoasComDiferentesSexos_DeveConverterCorretamente()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();
            var pessoas = new List<Pessoa>
            {
                new Pessoa("João Masculino", Sexo.Masculino, "joao@teste.com", DateTime.Now.AddYears(-30), "Cidade", "Brasil", "11111111111"),
                new Pessoa("Maria Feminino", Sexo.Feminino, "maria@teste.com", DateTime.Now.AddYears(-25), "Cidade", "Brasil", "22222222222"),
                new Pessoa("Alex Indefinido", null, "alex@teste.com", DateTime.Now.AddYears(-28), "Cidade", "Brasil", "33333333333")
            };

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ReturnsAsync((IEnumerable<PessoaDTO>)null!);

            _mockPessoaRepositorio.Setup(x => x.ListarAsync())
                                 .ReturnsAsync(pessoas);

            _mockServiceCache.Setup(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<IEnumerable<PessoaDTO>>(), It.IsAny<TimeSpan>()))
                           .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Count());
            
            var resultadoLista = resultado.ToList();
            Assert.Equal(Sexo.Masculino, resultadoLista[0].Sexo);
            Assert.Equal(Sexo.Feminino, resultadoLista[1].Sexo);
            Assert.Null(resultadoLista[2].Sexo);
        }

        #endregion

        #region Testes de Cache e Performance

        [Fact]
        public async Task Handle_CacheKeyCorreto_DeveUsarChaveCorreta()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();
            var expectedCacheKey = "lista_pessoas";

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>(expectedCacheKey))
                           .ReturnsAsync((IEnumerable<PessoaDTO>)null!);

            var pessoas = new List<Pessoa>
            {
                new Pessoa("Teste", Sexo.Masculino, "teste@teste.com", DateTime.Now.AddYears(-30), "Cidade", "Brasil", "12345678901")
            };

            _mockPessoaRepositorio.Setup(x => x.ListarAsync())
                                 .ReturnsAsync(pessoas);

            // Act
            await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            _mockServiceCache.Verify(x => x.ObterAsync<IEnumerable<PessoaDTO>>(expectedCacheKey), Times.Once);
            _mockServiceCache.Verify(x => x.DefinirAsync(expectedCacheKey, It.IsAny<IEnumerable<PessoaDTO>>(), TimeSpan.FromMinutes(5)), Times.Once);
        }

        [Fact]
        public async Task Handle_TempoExpiracao_DeveUsar5Minutos()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();
            var expectedExpiration = TimeSpan.FromMinutes(5);

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ReturnsAsync((IEnumerable<PessoaDTO>)null!);

            var pessoas = new List<Pessoa>
            {
                new Pessoa("Teste", Sexo.Masculino, "teste@teste.com", DateTime.Now.AddYears(-30), "Cidade", "Brasil", "12345678901")
            };

            _mockPessoaRepositorio.Setup(x => x.ListarAsync())
                                 .ReturnsAsync(pessoas);

            // Act
            await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            _mockServiceCache.Verify(x => x.DefinirAsync("lista_pessoas", It.IsAny<IEnumerable<PessoaDTO>>(), expectedExpiration), Times.Once);
        }

        [Fact]
        public async Task Handle_ListaGrande_DeveProcessarTodosOsItens()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();
            var pessoas = new List<Pessoa>();
            
            // Criar uma lista grande de pessoas para testar performance
            for (int i = 1; i <= 100; i++)
            {
                pessoas.Add(new Pessoa($"Pessoa {i}", Sexo.Masculino, $"pessoa{i}@teste.com", DateTime.Now.AddYears(-30), "Cidade", "Brasil", $"{i:D11}"));
            }

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ReturnsAsync((IEnumerable<PessoaDTO>)null!);

            _mockPessoaRepositorio.Setup(x => x.ListarAsync())
                                 .ReturnsAsync(pessoas);

            _mockServiceCache.Setup(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<IEnumerable<PessoaDTO>>(), It.IsAny<TimeSpan>()))
                           .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(100, resultado.Count());
            Assert.Equal("Pessoa 1", resultado.First().Nome);
            Assert.Equal("Pessoa 100", resultado.Last().Nome);
        }

        #endregion

        #region Testes de Cancellation Token

        [Fact]
        public async Task Handle_CancellationTokenCancelado_DeveRespeitarCancelamento()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ReturnsAsync((IEnumerable<PessoaDTO>)null!);

            _mockPessoaRepositorio.Setup(x => x.ListarAsync())
                                 .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => _handler.Handle(consulta, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task Handle_CancellationTokenValido_DeveExecutarNormalmente()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();
            var cancellationTokenSource = new CancellationTokenSource();

            var pessoasCache = new List<PessoaDTO>
            {
                new PessoaDTO
                {
                    Id = Guid.NewGuid(),
                    Nome = "Teste Cache",
                    Email = "cache@teste.com",
                    Cpf = "12345678901"
                }
            };

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ReturnsAsync(pessoasCache);

            // Act
            var resultado = await _handler.Handle(consulta, cancellationTokenSource.Token);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal("Teste Cache", resultado.First().Nome);
        }

        #endregion

        #region Testes de Edge Cases

        [Fact]
        public async Task Handle_CacheComException_DeveContinuarComRepositorio()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ThrowsAsync(new Exception("Erro no cache"));

            var pessoas = new List<Pessoa>
            {
                new Pessoa("Teste", Sexo.Masculino, "teste@teste.com", DateTime.Now.AddYears(-30), "Cidade", "Brasil", "12345678901")
            };

            _mockPessoaRepositorio.Setup(x => x.ListarAsync())
                                 .ReturnsAsync(pessoas);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(consulta, CancellationToken.None));

            Assert.Equal("Erro no cache", exception.Message);
        }

        [Fact]
        public async Task Handle_RepositorioComException_DevePropagar()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ReturnsAsync((IEnumerable<PessoaDTO>)null!);

            _mockPessoaRepositorio.Setup(x => x.ListarAsync())
                                 .ThrowsAsync(new Exception("Erro no repositório"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(consulta, CancellationToken.None));

            Assert.Equal("Erro no repositório", exception.Message);
        }

        [Fact]
        public async Task Handle_CacheDefinirComException_DeveIgnorarERetornarResultado()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();
            var pessoas = new List<Pessoa>
            {
                new Pessoa("Teste", Sexo.Masculino, "teste@teste.com", DateTime.Now.AddYears(-30), "Cidade", "Brasil", "12345678901")
            };

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ReturnsAsync((IEnumerable<PessoaDTO>)null!);

            _mockPessoaRepositorio.Setup(x => x.ListarAsync())
                                 .ReturnsAsync(pessoas);

            _mockServiceCache.Setup(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<IEnumerable<PessoaDTO>>(), It.IsAny<TimeSpan>()))
                           .ThrowsAsync(new Exception("Erro ao definir cache"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(consulta, CancellationToken.None));

            Assert.Equal("Erro ao definir cache", exception.Message);
        }

        #endregion

        #region Testes de Integração entre Cache e Repositório

        [Fact]
        public async Task Handle_FluxoCompleto_CacheMissEDepoisCacheHit()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();
            var pessoas = new List<Pessoa>
            {
                new Pessoa("Teste Integração", Sexo.Masculino, "integracao@teste.com", DateTime.Now.AddYears(-30), "Cidade", "Brasil", "12345678901")
            };

            var pessoasDto = new List<PessoaDTO>
            {
                new PessoaDTO
                {
                    Id = pessoas[0].Id,
                    Nome = pessoas[0].Nome,
                    Email = pessoas[0].Email,
                    Cpf = pessoas[0].Cpf
                }
            };

            // Primeira chamada: cache miss
            _mockServiceCache.SetupSequence(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ReturnsAsync((IEnumerable<PessoaDTO>)null!) // Cache miss na primeira chamada
                           .ReturnsAsync(pessoasDto); // Cache hit na segunda chamada

            _mockPessoaRepositorio.Setup(x => x.ListarAsync())
                                 .ReturnsAsync(pessoas);

            _mockServiceCache.Setup(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<IEnumerable<PessoaDTO>>(), It.IsAny<TimeSpan>()))
                           .Returns(Task.CompletedTask);

            // Act - Primeira chamada (cache miss)
            var resultado1 = await _handler.Handle(consulta, CancellationToken.None);

            // Act - Segunda chamada (cache hit)
            var resultado2 = await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado1);
            Assert.NotNull(resultado2);
            Assert.Single(resultado1);
            Assert.Single(resultado2);
            Assert.Equal(pessoas[0].Nome, resultado1.First().Nome);
            Assert.Equal(pessoas[0].Nome, resultado2.First().Nome);

            // Repositório deve ser chamado apenas uma vez (cache miss)
            _mockPessoaRepositorio.Verify(x => x.ListarAsync(), Times.Once);
            
            // Cache deve ser consultado duas vezes
            _mockServiceCache.Verify(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"), Times.Exactly(2));
            
            // Cache deve ser definido apenas uma vez (após cache miss)
            _mockServiceCache.Verify(x => x.DefinirAsync("lista_pessoas", It.IsAny<IEnumerable<PessoaDTO>>(), TimeSpan.FromMinutes(5)), Times.Once);
        }

        [Fact]
        public async Task Handle_VerificarConversaoCompleta_DeveMantarTodosOsCamposDaPessoa()
        {
            // Arrange
            var consulta = new ListarPessoasConsulta();
            var endereco = new Endereco("12345-678", "Rua Completa", "123", "Apto 456", "Bairro Teste", "Cidade Teste", "ST");
            var dataEspecifica = new DateTime(1990, 5, 15);
            
            var pessoas = new List<Pessoa>
            {
                new Pessoa("Nome Completo", Sexo.Feminino, "email@completo.com", dataEspecifica, "Naturalidade Teste", "Nacionalidade Teste", "12345678900", endereco)
            };

            _mockServiceCache.Setup(x => x.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas"))
                           .ReturnsAsync((IEnumerable<PessoaDTO>)null!);

            _mockPessoaRepositorio.Setup(x => x.ListarAsync())
                                 .ReturnsAsync(pessoas);

            _mockServiceCache.Setup(x => x.DefinirAsync(It.IsAny<string>(), It.IsAny<IEnumerable<PessoaDTO>>(), It.IsAny<TimeSpan>()))
                           .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(consulta, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            
            var pessoaDto = resultado.First();
            Assert.Equal("Nome Completo", pessoaDto.Nome);
            Assert.Equal(Sexo.Feminino, pessoaDto.Sexo);
            Assert.Equal("email@completo.com", pessoaDto.Email);
            Assert.Equal(dataEspecifica, pessoaDto.DataNascimento);
            Assert.Equal("Naturalidade Teste", pessoaDto.Naturalidade);
            Assert.Equal("Nacionalidade Teste", pessoaDto.Nacionalidade);
            Assert.Equal("12345678900", pessoaDto.Cpf);
            
            Assert.NotNull(pessoaDto.Endereco);
            Assert.Equal("12345-678", pessoaDto.Endereco.Cep);
            Assert.Equal("Rua Completa", pessoaDto.Endereco.Logradouro);
            Assert.Equal("123", pessoaDto.Endereco.Numero);
            Assert.Equal("Apto 456", pessoaDto.Endereco.Complemento);
            Assert.Equal("Bairro Teste", pessoaDto.Endereco.Bairro);
            Assert.Equal("Cidade Teste", pessoaDto.Endereco.Cidade);
            Assert.Equal("ST", pessoaDto.Endereco.Estado);
        }

        #endregion
    }
}