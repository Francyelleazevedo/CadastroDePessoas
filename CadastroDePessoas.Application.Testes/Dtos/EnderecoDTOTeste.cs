using CadastroDePessoas.Application.Dtos.Endereco;
using Xunit;

namespace CadastroDePessoas.Application.Testes.Dtos
{
    public class EnderecoDTOTeste
    {
        [Fact]
        public void EnderecoDTO_PropriedadesDevemSerCorretamenteSetadas()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cep = "12345-678";
            var logradouro = "Rua das Flores";
            var numero = "123";
            var complemento = "Apto 101";
            var bairro = "Centro";
            var cidade = "São Paulo";
            var estado = "SP";

            // Act
            var endereco = new EnderecoDTO
            {
                Id = id,
                Cep = cep,
                Logradouro = logradouro,
                Numero = numero,
                Complemento = complemento,
                Bairro = bairro,
                Cidade = cidade,
                Estado = estado
            };

            // Assert
            Assert.Equal(id, endereco.Id);
            Assert.Equal(cep, endereco.Cep);
            Assert.Equal(logradouro, endereco.Logradouro);
            Assert.Equal(numero, endereco.Numero);
            Assert.Equal(complemento, endereco.Complemento);
            Assert.Equal(bairro, endereco.Bairro);
            Assert.Equal(cidade, endereco.Cidade);
            Assert.Equal(estado, endereco.Estado);
        }

        [Fact]
        public void EnderecoDTO_ConstrutorPadrao_DeveCriarInstanciaValida()
        {
            // Act
            var endereco = new EnderecoDTO();

            // Assert
            Assert.NotNull(endereco);
            Assert.Equal(Guid.Empty, endereco.Id);
            Assert.Null(endereco.Cep);
            Assert.Null(endereco.Logradouro);
            Assert.Null(endereco.Numero);
            Assert.Null(endereco.Complemento);
            Assert.Null(endereco.Bairro);
            Assert.Null(endereco.Cidade);
            Assert.Null(endereco.Estado);
        }

        [Fact]
        public void EnderecoDTO_ToString_DeveRetornarEnderecoFormatado()
        {
            // Arrange
            var endereco = new EnderecoDTO
            {
                Logradouro = "Rua das Flores",
                Numero = "123",
                Complemento = "Apto 101",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                Cep = "12345-678"
            };

            // Act
            var resultado = endereco.ToString();

            // Assert
            var esperado = "Rua das Flores, 123, Apto 101, Centro, São Paulo - SP, 12345-678";
            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void EnderecoDTO_ToString_ComCamposNulos_DeveRetornarValoresDisponiveis()
        {
            // Arrange
            var endereco = new EnderecoDTO
            {
                Logradouro = "Rua das Flores",
                Numero = "123",
                Complemento = null,
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = null,
                Cep = "12345-678"
            };

            // Act
            var resultado = endereco.ToString();

            // Assert
            var esperado = "Rua das Flores, 123, Centro, São Paulo, 12345-678";
            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void EnderecoDTO_ToString_ComTodosCamposNulos_DeveRetornarStringVazia()
        {
            // Arrange
            var endereco = new EnderecoDTO
            {
                Logradouro = null,
                Numero = null,
                Complemento = null,
                Bairro = null,
                Cidade = null,
                Estado = null,
                Cep = null
            };

            // Act
            var resultado = endereco.ToString();

            // Assert
            Assert.Equal("", resultado);
        }

        [Fact]
        public void EnderecoDTO_EnderecoCompleto_DeveRetornarTrue_QuandoTodosOsCamposPrincipaisPreenchidos()
        {
            // Arrange
            var endereco = new EnderecoDTO
            {
                Logradouro = "Rua das Flores",
                Numero = "123",
                Complemento = "Apto 101",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                Cep = "12345-678"
            };

            // Act & Assert
            Assert.True(endereco.EnderecoCompleto);
        }

        [Fact]
        public void EnderecoDTO_EnderecoCompleto_DeveRetornarFalse_QuandoAlgumCampoPrincipalFaltando()
        {
            // Arrange
            var endereco = new EnderecoDTO
            {
                Logradouro = "Rua das Flores",
                Numero = "123",
                Complemento = "Apto 101",
                Bairro = null, // Campo principal faltando
                Cidade = "São Paulo",
                Estado = "SP",
                Cep = "12345-678"
            };

            // Act & Assert
            Assert.False(endereco.EnderecoCompleto);
        }

        [Fact]
        public void EnderecoDTO_EnderecoCompleto_DeveRetornarTrue_MesmoSemComplemento()
        {
            // Arrange
            var endereco = new EnderecoDTO
            {
                Logradouro = "Rua das Flores",
                Numero = "123",
                Complemento = null, // Complemento não é obrigatório
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                Cep = "12345-678"
            };

            // Act & Assert
            Assert.True(endereco.EnderecoCompleto);
        }
    }
}