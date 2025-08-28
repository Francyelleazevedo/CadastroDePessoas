
using System;
using Xunit;
using CadastroDePessoas.Domain.Entidades;

namespace CadastroDePessoas.Domain.Testes.Entidades
{
	public class EnderecoTeste
	{
		[Fact]
		public void Deve_Criar_Endereco_Com_Dados_Validos()
		{
			// Arrange & Act
			var endereco = new Endereco("12345-678", "Rua Teste", "100", "Apto 1", "Centro", "Cidade Teste", "UF");

			// Assert
			Assert.Equal("12345-678", endereco.Cep);
			Assert.Equal("Rua Teste", endereco.Logradouro);
			Assert.Equal("100", endereco.Numero);
			Assert.Equal("Apto 1", endereco.Complemento);
			Assert.Equal("Centro", endereco.Bairro);
			Assert.Equal("Cidade Teste", endereco.Cidade);
			Assert.Equal("UF", endereco.Estado);
		}

		[Fact]
		public void Deve_Atualizar_Endereco()
		{
			// Arrange
			var endereco = new Endereco("12345-678", "Rua Teste", "100", "Apto 1", "Centro", "Cidade Teste", "UF");

			// Act
			endereco.Atualizar("54321-000", "Rua Nova", "200", "Casa", "Bairro Novo", "Cidade Nova", "SP");

			// Assert
			Assert.Equal("54321-000", endereco.Cep);
			Assert.Equal("Rua Nova", endereco.Logradouro);
			Assert.Equal("200", endereco.Numero);
			Assert.Equal("Casa", endereco.Complemento);
			Assert.Equal("Bairro Novo", endereco.Bairro);
			Assert.Equal("Cidade Nova", endereco.Cidade);
			Assert.Equal("SP", endereco.Estado);
		}

		[Fact]
		public void DefinirPessoaId_Deve_Definir_Apenas_Se_Vazio()
		{
			// Arrange
			var endereco = new Endereco("12345-678", "Rua Teste", "100", "Apto 1", "Centro", "Cidade Teste", "UF");
			var pessoaId = Guid.NewGuid();

			// Act
			endereco.DefinirPessoaId(pessoaId);
			var primeiroId = endereco.PessoaId;
			endereco.DefinirPessoaId(Guid.NewGuid()); // Não deve sobrescrever

			// Assert
			Assert.Equal(pessoaId, endereco.PessoaId);
			Assert.Equal(primeiroId, endereco.PessoaId);
		}

		[Fact]
		public void ToString_Deve_Retornar_Endereco_Completo()
		{
			// Arrange
			var endereco = new Endereco("12345-678", "Rua Teste", "100", "Apto 1", "Centro", "Cidade Teste", "UF");

			// Act
			var texto = endereco.ToString();

			// Assert
			Assert.Contains("Rua Teste", texto);
			Assert.Contains("100", texto);
			Assert.Contains("Apto 1", texto);
			Assert.Contains("Centro", texto);
			Assert.Contains("Cidade Teste", texto);
			Assert.Contains("UF", texto);
			Assert.Contains("12345-678", texto);
		}
	}
}
