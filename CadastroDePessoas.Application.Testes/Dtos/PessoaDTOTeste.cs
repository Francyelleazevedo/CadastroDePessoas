using CadastroDePessoas.Application.Dtos.Endereco;
using CadastroDePessoas.Application.Dtos.Pessoa;
using CadastroDePessoas.Domain.Enums;
using Xunit;

namespace CadastroDePessoas.Application.Testes.Dtos
{
    public class PessoaDTOTeste
    {
        [Fact]
        public void PessoaDTO_PropriedadesDevemSerCorretamenteSetadas()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "João Silva";
            var sexo = Sexo.Masculino;
            var email = "joao@email.com";
            var dataNascimento = new DateTime(1990, 5, 15);
            var naturalidade = "São Paulo";
            var nacionalidade = "Brasileira";
            var cpf = "12345678901";
            var dataCadastro = DateTime.UtcNow;
            var dataAtualizacao = DateTime.UtcNow.AddDays(1);

            var endereco = new EnderecoDTO
            {
                Id = Guid.NewGuid(),
                Logradouro = "Rua das Flores",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                Cep = "12345-678"
            };

            // Act
            var pessoa = new PessoaDTO
            {
                Id = id,
                Nome = nome,
                Sexo = sexo,
                Email = email,
                DataNascimento = dataNascimento,
                Naturalidade = naturalidade,
                Nacionalidade = nacionalidade,
                Cpf = cpf,
                Endereco = endereco,
                DataCadastro = dataCadastro,
                DataAtualizacao = dataAtualizacao
            };

            // Assert
            Assert.Equal(id, pessoa.Id);
            Assert.Equal(nome, pessoa.Nome);
            Assert.Equal(sexo, pessoa.Sexo);
            Assert.Equal(email, pessoa.Email);
            Assert.Equal(dataNascimento, pessoa.DataNascimento);
            Assert.Equal(naturalidade, pessoa.Naturalidade);
            Assert.Equal(nacionalidade, pessoa.Nacionalidade);
            Assert.Equal(cpf, pessoa.Cpf);
            Assert.Equal(endereco, pessoa.Endereco);
            Assert.Equal(dataCadastro, pessoa.DataCadastro);
            Assert.Equal(dataAtualizacao, pessoa.DataAtualizacao);
        }

        [Fact]
        public void PessoaDTO_DevePermitirPropriedadesNulas()
        {
            // Act
            var pessoa = new PessoaDTO
            {
                Nome = "João Silva",
                Sexo = null, // Sexo pode ser nulo
                Email = "joao@email.com",
                DataNascimento = new DateTime(1990, 5, 15),
                Naturalidade = null, // Naturalidade pode ser nula
                Nacionalidade = null, // Nacionalidade pode ser nula
                Cpf = "12345678901",
                Endereco = null, // Endereço pode ser nulo
                DataAtualizacao = null // DataAtualizacao pode ser nula
            };

            // Assert
            Assert.NotNull(pessoa);
            Assert.Null(pessoa.Sexo);
            Assert.Null(pessoa.Naturalidade);
            Assert.Null(pessoa.Nacionalidade);
            Assert.Null(pessoa.Endereco);
            Assert.Null(pessoa.DataAtualizacao);
            Assert.Null(pessoa.DescricaoSexo); // Deve ser null quando Sexo é null
            Assert.Null(pessoa.EnderecoCompleto); // Deve ser null quando Endereco é null
        }

        [Fact]
        public void PessoaDTO_ConstrutorPadrao_DeveCriarInstanciaValida()
        {
            // Act
            var pessoa = new PessoaDTO();

            // Assert
            Assert.NotNull(pessoa);
            Assert.NotEqual(Guid.Empty, pessoa.Id); // O construtor gera um novo Guid
            Assert.Null(pessoa.Nome);
            Assert.Null(pessoa.Sexo);
            Assert.Null(pessoa.Email);
            Assert.Equal(DateTime.MinValue, pessoa.DataNascimento);
            Assert.Null(pessoa.Naturalidade);
            Assert.Null(pessoa.Nacionalidade);
            Assert.Null(pessoa.Cpf);
            Assert.Null(pessoa.Endereco);
            Assert.Equal(DateTime.MinValue, pessoa.DataCadastro);
            Assert.Null(pessoa.DataAtualizacao);
        }

        [Fact]
        public void PessoaDTO_EnderecoDTO_DeveSerCorretamenteSetado()
        {
            // Arrange
            var endereco = new EnderecoDTO
            {
                Id = Guid.NewGuid(),
                Logradouro = "Rua das Flores",
                Numero = "123",
                Complemento = "Apto 101",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                Cep = "12345-678"
            };

            var pessoa = new PessoaDTO();

            // Act
            pessoa.Endereco = endereco;

            // Assert
            Assert.NotNull(pessoa.Endereco);
            Assert.Equal(endereco.Id, pessoa.Endereco.Id);
            Assert.Equal(endereco.Logradouro, pessoa.Endereco.Logradouro);
            Assert.Equal(endereco.Numero, pessoa.Endereco.Numero);
            Assert.Equal(endereco.Complemento, pessoa.Endereco.Complemento);
            Assert.Equal(endereco.Bairro, pessoa.Endereco.Bairro);
            Assert.Equal(endereco.Cidade, pessoa.Endereco.Cidade);
            Assert.Equal(endereco.Estado, pessoa.Endereco.Estado);
            Assert.Equal(endereco.Cep, pessoa.Endereco.Cep);
            
            // Verifica se o EnderecoCompleto é formatado corretamente
            var enderecoCompletoEsperado = $"{endereco.Logradouro}, {endereco.Numero}, {endereco.Bairro}, {endereco.Cidade} - {endereco.Estado}, {endereco.Cep}";
            Assert.Equal(enderecoCompletoEsperado, pessoa.EnderecoCompleto);
        }

        [Fact]
        public void PessoaDTO_DescricaoSexo_DeveRetornarStringCorreto()
        {
            // Arrange & Act
            var pessoaMasculino = new PessoaDTO { Sexo = Sexo.Masculino };
            var pessoaFeminino = new PessoaDTO { Sexo = Sexo.Feminino };
            var pessoaNulo = new PessoaDTO { Sexo = null };

            // Assert
            Assert.Equal("Masculino", pessoaMasculino.DescricaoSexo);
            Assert.Equal("Feminino", pessoaFeminino.DescricaoSexo);
            Assert.Null(pessoaNulo.DescricaoSexo);
        }

        [Fact]
        public void PessoaDTO_EnderecoCompleto_ComEnderecoNulo_DeveRetornarNull()
        {
            // Arrange
            var pessoa = new PessoaDTO
            {
                Nome = "João Silva",
                Endereco = null
            };

            // Act & Assert
            Assert.Null(pessoa.EnderecoCompleto);
        }
    }
}