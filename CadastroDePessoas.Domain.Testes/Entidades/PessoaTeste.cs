
using System;
using Xunit;
using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Domain.Enums;

namespace CadastroDePessoas.Domain.Testes.Entidades
{
	public class PessoaTeste
	{
		[Fact]
		public void Deve_Criar_Pessoa_Com_Dados_Validos()
		{
			// Arrange
			var endereco = new Endereco("12345-678", "Rua Teste", "100", "Apto 1", "Centro", "Cidade Teste", "UF");
			var dataNascimento = new DateTime(1990, 1, 1);

			// Act
			var pessoa = new Pessoa(
				nome: "João Silva",
				sexo: Sexo.Masculino,
				email: "joao@email.com",
				dataNascimento: dataNascimento,
				naturalidade: "Cidade Natal",
				nacionalidade: "Brasileira",
				cpf: "12345678901",
				endereco: endereco
			);

			// Assert
			Assert.Equal("João Silva", pessoa.Nome);
			Assert.Equal(Sexo.Masculino, pessoa.Sexo);
			Assert.Equal("joao@email.com", pessoa.Email);
			Assert.Equal(dataNascimento, pessoa.DataNascimento);
			Assert.Equal("Cidade Natal", pessoa.Naturalidade);
			Assert.Equal("Brasileira", pessoa.Nacionalidade);
			Assert.Equal("12345678901", pessoa.Cpf);
			Assert.NotNull(pessoa.Endereco);
			Assert.Equal(pessoa.Id, pessoa.Endereco.PessoaId);
		}

		[Fact]
		public void Deve_Atualizar_Dados_Pessoais()
		{
			// Arrange
			var pessoa = new Pessoa("Maria", Sexo.Feminino, "maria@email.com", new DateTime(1995, 5, 5), "Cidade", "Brasileira", "98765432100");
			var novaData = new DateTime(2000, 10, 10);

			// Act
			pessoa.Atualizar("Maria Souza", Sexo.Feminino, "maria.souza@email.com", novaData, "Nova Cidade", "Brasileira");

			// Assert
			Assert.Equal("Maria Souza", pessoa.Nome);
			Assert.Equal("maria.souza@email.com", pessoa.Email);
			Assert.Equal(novaData, pessoa.DataNascimento);
			Assert.Equal("Nova Cidade", pessoa.Naturalidade);
		}

		[Fact]
		public void Deve_Atualizar_Endereco()
		{
			// Arrange
			var pessoa = new Pessoa("Carlos", Sexo.Masculino, "carlos@email.com", new DateTime(1980, 3, 3), "Cidade", "Brasileira", "11122233344");
			var novoEndereco = new Endereco("99999-999", "Av. Nova", "200", "Casa", "Bairro Novo", "Cidade Nova", "UF");

			// Act
			pessoa.AtualizarEndereco(novoEndereco);

			// Assert
			Assert.NotNull(pessoa.Endereco);
			Assert.Equal("Av. Nova", pessoa.Endereco.Logradouro);
			Assert.Equal(pessoa.Id, pessoa.Endereco.PessoaId);
		}
	}
}
