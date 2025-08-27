using CadastroDePessoas.Application.Dtos.Endereco;
using CadastroDePessoas.Application.Dtos.Pessoa;
using CadastroDePessoas.Domain.Enums;
using MediatR;

namespace CadastroDePessoas.Application.CQRS.Comandos.Pessoa.AtualizarPessoa
{
    public class AtualizarPessoaComando : IRequest<PessoaDTO>
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public Sexo? Sexo { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
        public string Cpf { get; set; }
        public AtualizarEnderecoComando? Endereco { get; set; }

        public AtualizarPessoaDTO ParaDTO()
        {
            return new AtualizarPessoaDTO
            {
                Id = Id,
                Nome = Nome,
                Sexo = Sexo,
                Email = Email,
                DataNascimento = DataNascimento,
                Naturalidade = Naturalidade,
                Nacionalidade = Nacionalidade,
                Cpf = Cpf,  
                Endereco = Endereco?.ParaDTO()
            };
        }
    }

    public class AtualizarEnderecoComando
    {
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }

       public CriarEnderecoDTO ParaDTO()
        {
            return new CriarEnderecoDTO
            {
                Cep = Cep,
                Logradouro = Logradouro,
                Numero = Numero,
                Complemento = Complemento,
                Bairro = Bairro,
                Cidade = Cidade,
                Estado = Estado
            };
        }
    }
}
