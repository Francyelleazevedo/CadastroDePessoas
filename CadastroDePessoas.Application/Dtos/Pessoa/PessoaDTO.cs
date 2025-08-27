using CadastroDePessoas.Application.Dtos.Endereco;
using CadastroDePessoas.Domain.Enums;

namespace CadastroDePessoas.Application.Dtos.Pessoa
{
    public class PessoaDTO
    {
        public PessoaDTO()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Nome { get; set; }
        public Sexo? Sexo { get; set; }
        public string DescricaoSexo => Sexo?.ToString();
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
        public string Cpf { get; set; }
        public EnderecoDTO Endereco { get; set; }
        public string EnderecoCompleto => Endereco != null ? $"{Endereco.Logradouro}, {Endereco.Numero}, {Endereco.Bairro}, {Endereco.Cidade} - {Endereco.Estado}, {Endereco.Cep}" :
            null;
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
