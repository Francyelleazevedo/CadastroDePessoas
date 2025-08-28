
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa;
using CadastroDePessoas.Application.Dtos.Pessoa;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Domain.Interfaces;
using PessoaEntidade = CadastroDePessoas.Domain.Entidades.Pessoa;

namespace CadastroDePessoas.Application.Testes.Comandos.Pessoa
{
	public class CriarPessoaComandoTeste
	{
		private readonly Mock<IPessoaRepositorio> _repositorioPessoaMock;
		private readonly Mock<IServiceCache> _servicoCacheMock;
		private readonly CriarPessoaComandoHandler _manipulador;

		public CriarPessoaComandoTeste()
		{
			_repositorioPessoaMock = new Mock<IPessoaRepositorio>();
			_servicoCacheMock = new Mock<IServiceCache>();
			_manipulador = new CriarPessoaComandoHandler(_repositorioPessoaMock.Object, _servicoCacheMock.Object);
		}

		[Fact]
		public async Task Handle_ComDadosValidos_DeveCriarPessoa()
		{
			// Arrange
			var endereco = new EnderecoComando
			{
				Cep = "01310-100",
				Logradouro = "Av. Boa Viagem",
				Numero = "1000",
				Complemento = "Apto 101",
				Bairro = "Bela Vista",
				Cidade = "Recife",
				Estado = "PE"
			};

			var comando = new CriarPessoaComando
			{
				Nome = "João Silva",
				Sexo = Sexo.Masculino,
				Email = "joao@exemplo.com",
				DataNascimento = new DateTime(1990, 1, 1),
				Naturalidade = "Recife",
				Nacionalidade = "Brasileira",
				Cpf = "21815043008",
			Endereco = endereco
		};

		_repositorioPessoaMock
			.Setup(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
			.ReturnsAsync(false);

		_repositorioPessoaMock
			.Setup(r => r.AdicionarAsync(It.IsAny<PessoaEntidade>()))
			.Returns(Task.CompletedTask);			// Act
			var resultado = await _manipulador.Handle(comando, CancellationToken.None);

			// Assert
			Assert.NotNull(resultado);
			Assert.IsType<PessoaDTO>(resultado);
			Assert.Equal(comando.Nome, resultado.Nome);
			Assert.Equal(comando.Sexo, resultado.Sexo);
			Assert.Equal(comando.Email, resultado.Email);
			Assert.Equal(comando.DataNascimento, resultado.DataNascimento);
			Assert.Equal(comando.Naturalidade, resultado.Naturalidade);
			Assert.Equal(comando.Nacionalidade, resultado.Nacionalidade);
			Assert.Equal(comando.Cpf, resultado.Cpf);
			Assert.NotNull(resultado.Endereco);
			Assert.Equal(endereco.Cep, resultado.Endereco.Cep);
			Assert.Equal(endereco.Logradouro, resultado.Endereco.Logradouro);
			Assert.Equal(endereco.Numero, resultado.Endereco.Numero);
			Assert.Equal(endereco.Cidade, resultado.Endereco.Cidade);

			_repositorioPessoaMock.Verify(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Once);
			_repositorioPessoaMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entidades.Pessoa>()), Times.Once);
			_servicoCacheMock.Verify(c => c.RemoverAsync("lista_pessoas"), Times.Once);
		}

		[Fact]
		public async Task Handle_SemEndereco_DeveCriarPessoaSemEndereco()
		{
			// Arrange
			var comando = new CriarPessoaComando
			{
				Nome = "João Silva",
				Sexo = Sexo.Masculino,
				Email = "joao@exemplo.com",
				DataNascimento = new DateTime(1990, 1, 1),
				Naturalidade = "Recife",
				Nacionalidade = "Brasileira",
				Cpf = "21815043008",
			Endereco = null
		};

		_repositorioPessoaMock
			.Setup(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
			.ReturnsAsync(false);

		_repositorioPessoaMock
			.Setup(r => r.AdicionarAsync(It.IsAny<PessoaEntidade>()))
			.Returns(Task.CompletedTask);			// Act
			var resultado = await _manipulador.Handle(comando, CancellationToken.None);

			// Assert
			Assert.NotNull(resultado);
			Assert.Equal(comando.Nome, resultado.Nome);
			Assert.Equal(comando.Cpf, resultado.Cpf);
			Assert.Null(resultado.Endereco);

			_repositorioPessoaMock.Verify(r => r.AdicionarAsync(It.IsAny<PessoaEntidade>()), Times.Once);
		}

		[Fact]
		public async Task Handle_ComCPFExistente_DeveLancarExcecao()
		{
			// Arrange
			var comando = new CriarPessoaComando
			{
				Nome = "João Silva",
				Sexo = Sexo.Masculino,
				Email = "joao@exemplo.com",
				DataNascimento = new DateTime(1990, 1, 1),
				Naturalidade = "Recife",
				Nacionalidade = "Brasileira",
				Cpf = "21815043008"
			};

			_repositorioPessoaMock
				.Setup(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
				.ReturnsAsync(true);

			// Act & Assert
			var excecao = await Assert.ThrowsAsync<Exception>(() => _manipulador.Handle(comando, CancellationToken.None));
			Assert.Equal("CPF já está em uso.", excecao.Message);

			_repositorioPessoaMock.Verify(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Once);
			_repositorioPessoaMock.Verify(r => r.AdicionarAsync(It.IsAny<PessoaEntidade>()), Times.Never);
			_servicoCacheMock.Verify(c => c.RemoverAsync("lista_pessoas"), Times.Never);
		}
	}
}
