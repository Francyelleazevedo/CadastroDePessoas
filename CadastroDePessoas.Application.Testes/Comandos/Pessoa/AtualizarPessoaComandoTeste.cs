using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.AtualizarPessoa;
using CadastroDePessoas.Application.Dtos.Pessoa;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Domain.Interfaces;
using PessoaEntidade = CadastroDePessoas.Domain.Entidades.Pessoa;

namespace CadastroDePessoas.Application.Testes.Comandos.Pessoa
{
    public class AtualizarPessoaComandoTeste
    {
        private readonly Mock<IPessoaRepositorio> _repositorioPessoaMock;
        private readonly Mock<IServiceCache> _servicoCacheMock;
        private readonly AtualizarPessoaComandoHandler _manipulador;
        private readonly Guid _pessoaId = Guid.NewGuid();

        public AtualizarPessoaComandoTeste()
        {
            _repositorioPessoaMock = new Mock<IPessoaRepositorio>();
            _servicoCacheMock = new Mock<IServiceCache>();
            _manipulador = new AtualizarPessoaComandoHandler(_repositorioPessoaMock.Object, _servicoCacheMock.Object);
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveAtualizarPessoa()
        {
            // Arrange
            var endereco = new CadastroDePessoas.Domain.Entidades.Endereco(
                "01310-100",
                "Av. Boa Viagem",
                "1000",
                "Apto 101",
                "Bela Vista",
                "Recife",
                "PE"
            );

            var pessoaExistente = new PessoaEntidade(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "21815043008",
                endereco
            );

            var enderecoAtualizado = new AtualizarEnderecoComando
            {
                Cep = "04538-132",
                Logradouro = "Av. Felicidade",
                Numero = "3900",
                Complemento = "Andar 1",
                Bairro = "Madalena",
                Cidade = "Recife",
                Estado = "PE"
            };

            var comando = new AtualizarPessoaComando
            {
                Id = _pessoaId,
                Nome = "João Silva Atualizado",
                Sexo = Sexo.Masculino,
                Email = "joao.atualizado@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                Naturalidade = "Rio de Janeiro",
                Nacionalidade = "Brasileira",
                Cpf = "21815043008",
                Endereco = enderecoAtualizado
            };

            _repositorioPessoaMock
                .Setup(r => r.ObterPorIdAsync(_pessoaId))
                .ReturnsAsync(pessoaExistente);

            _repositorioPessoaMock
                .Setup(r => r.AtualizarPessoaComEnderecoAsync(It.IsAny<PessoaEntidade>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _manipulador.Handle(comando, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<PessoaDTO>(resultado);
            Assert.Equal(comando.Nome, resultado.Nome);
            Assert.Equal(comando.Email, resultado.Email);
            Assert.Equal(comando.Naturalidade, resultado.Naturalidade);
            Assert.NotNull(resultado.Endereco);
            Assert.Equal(enderecoAtualizado.Cep, resultado.Endereco.Cep);
            Assert.Equal(enderecoAtualizado.Logradouro, resultado.Endereco.Logradouro);
            Assert.Equal(enderecoAtualizado.Numero, resultado.Endereco.Numero);
            Assert.Equal(enderecoAtualizado.Cidade, resultado.Endereco.Cidade);

            _repositorioPessoaMock.Verify(r => r.ObterPorIdAsync(_pessoaId), Times.Once);
            _repositorioPessoaMock.Verify(r => r.AtualizarPessoaComEnderecoAsync(It.IsAny<PessoaEntidade>()), Times.Once);
            _servicoCacheMock.Verify(c => c.RemoverAsync("lista_pessoas"), Times.Once);
            _servicoCacheMock.Verify(c => c.RemoverAsync($"pessoa_{_pessoaId}"), Times.Once);
        }

        [Fact]
        public async Task Handle_SemAtualizarEndereco_DeveManterEnderecoExistente()
        {
            // Arrange
            var endereco = new CadastroDePessoas.Domain.Entidades.Endereco(
                "01310-100",
                "Av. Boa Viagem",
                "1000",
                "Apto 101",
                "Bela Vista",
                "Recife",
                "PE"
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

            var comando = new AtualizarPessoaComando
            {
                Id = _pessoaId,
                Nome = "João Silva Atualizado",
                Sexo = Sexo.Masculino,
                Email = "joao.atualizado@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                Naturalidade = "Rio de Janeiro",
                Nacionalidade = "Brasileira",
                Cpf = "52998224725",
                Endereco = null
            };

            _repositorioPessoaMock
                .Setup(r => r.ObterPorIdAsync(_pessoaId))
                .ReturnsAsync(pessoaExistente);

            _repositorioPessoaMock
                .Setup(r => r.AtualizarPessoaComEnderecoAsync(It.IsAny<PessoaEntidade>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _manipulador.Handle(comando, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(comando.Nome, resultado.Nome);
            Assert.Equal(comando.Email, resultado.Email);

            // O endereço deve ser mantido como estava
            Assert.NotNull(resultado.Endereco);
            Assert.Equal("01310-100", resultado.Endereco.Cep);
            Assert.Equal("Av. Boa Viagem", resultado.Endereco.Logradouro);
        }

        [Fact]
        public async Task Handle_ComPessoaInexistente_DeveLancarExcecao()
        {
            // Arrange
            var comando = new AtualizarPessoaComando
            {
                Id = _pessoaId,
                Nome = "João Silva Atualizado",
                Sexo = Sexo.Masculino,
                Email = "joao.atualizado@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                Naturalidade = "Rio de Janeiro",
                Nacionalidade = "Brasileira",
                Cpf = "52998224725"
            };

            _repositorioPessoaMock
                .Setup(r => r.ObterPorIdAsync(_pessoaId))
                .ReturnsAsync((PessoaEntidade?)null);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<Exception>(() => _manipulador.Handle(comando, CancellationToken.None));
            Assert.Equal("Pessoa não encontrada", excecao.Message);

            _repositorioPessoaMock.Verify(r => r.ObterPorIdAsync(_pessoaId), Times.Once);
            _repositorioPessoaMock.Verify(r => r.AtualizarPessoaComEnderecoAsync(It.IsAny<PessoaEntidade>()), Times.Never);
            _servicoCacheMock.Verify(c => c.RemoverAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
