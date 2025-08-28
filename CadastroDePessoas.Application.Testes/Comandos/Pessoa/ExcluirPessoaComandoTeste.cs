
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.ExcluirPessoa;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Domain.Interfaces;
using PessoaEntidade = CadastroDePessoas.Domain.Entidades.Pessoa;

namespace CadastroDePessoas.Application.Testes.Comandos.Pessoa
{
	public class ExcluirPessoaComandoTeste
	{
		private readonly Mock<IPessoaRepositorio> _repositorioPessoaMock;
		private readonly Mock<IServiceCache> _servicoCacheMock;
		private readonly ExcluirPessoaComandoHandler _manipulador;
		private readonly Guid _pessoaId = Guid.NewGuid();

		public ExcluirPessoaComandoTeste()
		{
			_repositorioPessoaMock = new Mock<IPessoaRepositorio>();
			_servicoCacheMock = new Mock<IServiceCache>();
			_manipulador = new ExcluirPessoaComandoHandler(_repositorioPessoaMock.Object, _servicoCacheMock.Object);
		}

		[Fact]
		public async Task Handle_ComPessoaExistente_DeveExcluirPessoa()
		{
			// Arrange
			var endereco = new Endereco(
				"01310-100",
				"Av. Paulista",
				"1000",
				"Apto 101",
				"Bela Vista",
				"São Paulo",
				"SP"
			);

			var pessoaExistente = new PessoaEntidade(
				"João Silva",
				Sexo.Masculino,
				"joao@exemplo.com",
				new DateTime(1990, 1, 1),
				"São Paulo",
				"Brasileira",
				"52998224725",
				endereco
			);

			var comando = new ExcluirPessoaComando(_pessoaId);

			_repositorioPessoaMock
				.Setup(r => r.ObterPorIdAsync(_pessoaId))
				.ReturnsAsync(pessoaExistente);

			_repositorioPessoaMock
				.Setup(r => r.RemoverAsync(It.IsAny<PessoaEntidade>()))
				.Returns(Task.CompletedTask);

			// Act
			var resultado = await _manipulador.Handle(comando, CancellationToken.None);

			// Assert
			Assert.True(resultado);

			_repositorioPessoaMock.Verify(r => r.ObterPorIdAsync(_pessoaId), Times.Once);
			_repositorioPessoaMock.Verify(r => r.RemoverAsync(It.IsAny<PessoaEntidade>()), Times.Once);
			_servicoCacheMock.Verify(c => c.RemoverAsync("lista_pessoas"), Times.Once);
			_servicoCacheMock.Verify(c => c.RemoverAsync($"pessoa_{_pessoaId}"), Times.Once);
		}

		[Fact]
		public async Task Handle_ComPessoaSemEndereco_DeveExcluirPessoa()
		{
			// Arrange
			var pessoaExistente = new PessoaEntidade(
				"João Silva",
				Sexo.Masculino,
				"joao@exemplo.com",
				new DateTime(1990, 1, 1),
				"São Paulo",
				"Brasileira",
				"52998224725",
				null
			);

			var comando = new ExcluirPessoaComando(_pessoaId);

			_repositorioPessoaMock
				.Setup(r => r.ObterPorIdAsync(_pessoaId))
				.ReturnsAsync(pessoaExistente);

			_repositorioPessoaMock
				.Setup(r => r.RemoverAsync(It.IsAny<PessoaEntidade>()))
				.Returns(Task.CompletedTask);

			// Act
			var resultado = await _manipulador.Handle(comando, CancellationToken.None);

			// Assert
			Assert.True(resultado);

			_repositorioPessoaMock.Verify(r => r.ObterPorIdAsync(_pessoaId), Times.Once);
			_repositorioPessoaMock.Verify(r => r.RemoverAsync(It.IsAny<PessoaEntidade>()), Times.Once);
		}

		[Fact]
		public async Task Handle_ComPessoaInexistente_DeveLancarExcecao()
		{
			// Arrange
			var comando = new ExcluirPessoaComando(_pessoaId);

			_repositorioPessoaMock
				.Setup(r => r.ObterPorIdAsync(_pessoaId))
				.ReturnsAsync((PessoaEntidade)null);

			// Act & Assert
			var excecao = await Assert.ThrowsAsync<Exception>(() => _manipulador.Handle(comando, CancellationToken.None));
			Assert.Equal("Pessoa não encontrada", excecao.Message);

			_repositorioPessoaMock.Verify(r => r.ObterPorIdAsync(_pessoaId), Times.Once);
			_repositorioPessoaMock.Verify(r => r.RemoverAsync(It.IsAny<PessoaEntidade>()), Times.Never);
			_servicoCacheMock.Verify(c => c.RemoverAsync(It.IsAny<string>()), Times.Never);
		}
	}
}
