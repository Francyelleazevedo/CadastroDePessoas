using CadastroDePessoas.Application.Dtos.Endereco;
using CadastroDePessoas.Application.Dtos.Pessoa;
using CadastroDePessoas.Domain.Entidades;

namespace CadastroDePessoas.Application.Factories
{
    public static class PessoaFactory
    {
        public static Pessoa CriarEntidade(CriarPessoaDTO dto)
        {
            var endereco = dto.Endereco != null ?
                new Endereco(
                    dto.Endereco.Cep,
                    dto.Endereco.Logradouro,
                    dto.Endereco.Numero,
                    dto.Endereco.Complemento,
                    dto.Endereco.Bairro,
                    dto.Endereco.Cidade,
                    dto.Endereco.Estado
                ) : null;

            return new Pessoa(
                dto.Nome,
                dto.Sexo,
                dto.Email,
                dto.DataNascimento,
                dto.Naturalidade,
                dto.Nacionalidade,
                dto.Cpf,
                endereco
            );
        }

        public static PessoaDTO CriarDTO(Pessoa entidade)
        {
            var enderecoDto = entidade.Endereco != null ?
                new EnderecoDTO
                {
                    Id = entidade.Endereco.Id,
                    Cep = entidade.Endereco.Cep,
                    Logradouro = entidade.Endereco.Logradouro,
                    Numero = entidade.Endereco.Numero,
                    Complemento = entidade.Endereco.Complemento,
                    Bairro = entidade.Endereco.Bairro,
                    Cidade = entidade.Endereco.Cidade,
                    Estado = entidade.Endereco.Estado
                } : null;

            return new PessoaDTO
            {
                Id = entidade.Id,
                Nome = entidade.Nome,
                Sexo = entidade.Sexo,
                Email = entidade.Email,
                DataNascimento = entidade.DataNascimento,
                Naturalidade = entidade.Naturalidade,
                Nacionalidade = entidade.Nacionalidade,
                Cpf = entidade.Cpf,
                Endereco = enderecoDto,
                DataCadastro = entidade.DataCadastro,
                DataAtualizacao = entidade.DataAtualizacao
            };
        }
    }
}
