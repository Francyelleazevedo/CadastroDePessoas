using CadastroDePessoas.Application.Dtos.Endereco;
using CadastroDePessoas.Domain.Enums;

namespace CadastroDePessoas.Application.Dtos.Pessoa
{
    public class CriarPessoaDTO
    {
        public string Nome { get; set; }
        public Sexo? Sexo { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
        public string Cpf { get; set; }
        public CriarEnderecoDTO Endereco { get; set; }
    }
}
