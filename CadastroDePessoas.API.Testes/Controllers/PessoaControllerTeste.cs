using System.Text;
using CadastroDePessoas.API.Controllers;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.AtualizarPessoa;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.ExcluirPessoa;
using CadastroDePessoas.Application.CQRS.Consultas.ListarPessoas;
using CadastroDePessoas.Application.CQRS.Consultas.ObterPessoa;
using CadastroDePessoas.Application.Dtos.Pessoa;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CadastroDePessoas.API.Testes.Controllers
{
    public class PessoasControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<PessoaController>> _loggerMock;
        private readonly Mock<IServiceCache> _serviceCacheMock;
        private readonly PessoaController _controller;

        public PessoasControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<PessoaController>>();
            _serviceCacheMock = new Mock<IServiceCache>();

            _controller = new PessoaController(
                _mediatorMock.Object,
                _loggerMock.Object,
                _serviceCacheMock.Object
            );
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarOk()
        {
            // Arrange
            var pessoasLista = new List<PessoaDTO>
            {
                new PessoaDTO
                {
                    Id = Guid.NewGuid(),
                    Nome = "Maria José",
                    Sexo = Sexo.Feminino,
                    Email = "maria@exemplo.com",
                    DataNascimento = new DateTime(1990, 1, 1),
                    Cpf = "52998224725"
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ListarPessoasConsulta>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pessoasLista);

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == "lista_pessoas")))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.ObterTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var pessoas = Assert.IsAssignableFrom<IEnumerable<PessoaDTO>>(okResult.Value);
            Assert.Single(pessoas);
            
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == "pessoasListaCacheKey")), Times.Once);
        }

        [Fact]
        public async Task ObterTodos_QuandoOcorreExcecao_DeveRetornarStatusCode500()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ListarPessoasConsulta>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro ao buscar pessoas"));

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == "lista_pessoas")))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.ObterTodos();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var chaveCache = $"pessoa_{id}";
            var pessoa = new PessoaDTO
            {
                Id = id,
                Nome = "Maria José",
                Sexo = Sexo.Feminino,
                Email = "maria@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                Cpf = "52998224725"
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPessoaConsulta>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pessoa);

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCache)))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.ObterPorId(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var pessoaRetornada = Assert.IsType<PessoaDTO>(okResult.Value);
            Assert.Equal(id, pessoaRetornada.Id);
            
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == chaveCache)), Times.Once);
            
            _loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task ObterPorId_QuandoPessoaNaoEncontrada_DeveRetornarNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var chaveCache = $"pessoa_{id}";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPessoaConsulta>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PessoaDTO?)null);

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCache)))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.ObterPorId(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado.Result);
            Assert.Contains($"ID {id}", notFoundResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task ObterPorId_QuandoOcorreExcecao_DeveRetornarStatusCode500()
        {
            // Arrange
            var id = Guid.NewGuid();
            var chaveCache = $"pessoa_{id}";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPessoaConsulta>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro ao buscar pessoa"));

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCache)))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.ObterPorId(id);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Criar_DeveRetornarCreated()
        {
            // Arrange
            var comando = new CriarPessoaComando
            {
                Nome = "Maria José",
                Sexo = Sexo.Feminino,
                Email = "maria@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                Cpf = "52998224725"
            };

            var pessoaCriada = new PessoaDTO
            {
                Id = Guid.NewGuid(),
                Nome = comando.Nome,
                Sexo = comando.Sexo,
                Email = comando.Email,
                DataNascimento = comando.DataNascimento,
                Cpf = comando.Cpf
            };

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pessoaCriada);

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == "lista_pessoas")))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.Criar(comando);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var pessoa = Assert.IsType<PessoaDTO>(createdAtResult.Value);
            Assert.Equal(comando.Nome, pessoa.Nome);
            Assert.Equal("ObterPorId", createdAtResult.ActionName);
            
            if (createdAtResult.RouteValues != null && createdAtResult.RouteValues.TryGetValue("id", out var routeValue))
            {
                Assert.Equal(pessoaCriada.Id, routeValue);
            }
            else
            {
                Assert.Fail("RouteValues['id'] não encontrado");
            }
            
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == "lista_pessoas")), Times.Once);
        }

        [Fact]
        public async Task Criar_QuandoOcorreExcecao_DeveRetornarBadRequest()
        {
            // Arrange
            var comando = new CriarPessoaComando
            {
                Nome = "Maria José",
                Cpf = "CPF_Invalido"
            };

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("CPF inválido"));

            // Act
            var resultado = await _controller.Criar(comando);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.Contains("Erro ao criar pessoa", badRequestResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarOk()
        {
            // Arrange
            var comando = new AtualizarPessoaComando
            {
                Id = Guid.NewGuid(),
                Nome = "Maria José Atualizado",
                Sexo = Sexo.Feminino,
                Email = "joao.atualizado@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                Naturalidade = "Rio de Janeiro",
                Nacionalidade = "Brasileira"
            };

            var pessoaAtualizada = new PessoaDTO
            {
                Id = comando.Id,
                Nome = comando.Nome,
                Sexo = comando.Sexo,
                Email = comando.Email,
                DataNascimento = comando.DataNascimento,
                Naturalidade = comando.Naturalidade,
                Nacionalidade = comando.Nacionalidade
            };

            var chaveCachePessoa = $"pessoa_{comando.Id}";
            var chaveCacheLista = "lista_pessoas";

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pessoaAtualizada);

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCachePessoa)))
                .Returns(Task.CompletedTask);
                
            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.Atualizar(comando);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var pessoa = Assert.IsType<PessoaDTO>(okResult.Value);
            Assert.Equal(comando.Nome, pessoa.Nome);
            Assert.Equal(comando.Email, pessoa.Email);
            
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == chaveCachePessoa)), Times.Once);
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)), Times.Once);
        }

        [Fact]
        public async Task Atualizar_QuandoOcorreExcecao_DeveRetornarBadRequest()
        {
            // Arrange
            var comando = new AtualizarPessoaComando
            {
                Id = Guid.NewGuid(),
                Nome = "Maria José Atualizado"
            };

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Pessoa n�o encontrada"));

            // Act
            var resultado = await _controller.Atualizar(comando);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.Contains("Erro ao atualizar pessoa", badRequestResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task Remover_DeveRetornarOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var chaveCachePessoa = $"pessoa_{id}";
            var chaveCacheLista = "lista_pessoas";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ExcluirPessoaComando>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCachePessoa)))
                .Returns(Task.CompletedTask);
                
            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.Remover(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var sucesso = Assert.IsType<bool>(okResult.Value);
            Assert.True(sucesso);
            
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == chaveCachePessoa)), Times.Once);
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)), Times.Once);
        }

        [Fact]
        public async Task Remover_QuandoOcorreExcecao_DeveRetornarBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ExcluirPessoaComando>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Pessoa n�o encontrada"));

            // Act
            var resultado = await _controller.Remover(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.Contains("Erro ao remover pessoa", badRequestResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task LimparCache_DeveRetornarOk()
        {
            // Arrange
            var chaveCacheLista = "lista_pessoas";
            
            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.LimparCache();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var response = Assert.IsAssignableFrom<object>(okResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "message");
            
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)), Times.Once);
        }

        [Fact]
        public async Task LimparCache_QuandoOcorreExcecao_DeveRetornarStatusCode500()
        {
            // Arrange
            var chaveCacheLista = "lista_pessoas";
            
            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)))
                .ThrowsAsync(new Exception("Erro ao limpar cache"));

            // Act
            var resultado = await _controller.LimparCache();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Erro ao limpar cache", statusCodeResult.Value?.ToString() ?? string.Empty);
        }
    }
}