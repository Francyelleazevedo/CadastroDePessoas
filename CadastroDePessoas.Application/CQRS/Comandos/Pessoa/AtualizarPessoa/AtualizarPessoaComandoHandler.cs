using CadastroDePessoas.Application.Dtos.Pessoa;
using CadastroDePessoas.Application.Factories;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Domain.Interfaces;

namespace CadastroDePessoas.Application.CQRS.Comandos.Pessoa.AtualizarPessoa
{
    public class AtualizarPessoaComandoHandler(IPessoaRepositorio pessoaRepositorio, IServiceCache servicoCache) : IHandlerComando<AtualizarPessoaComando, PessoaDTO>
    {
        public async Task<PessoaDTO> Handle(AtualizarPessoaComando comando, CancellationToken cancellationToken)
        {
            var pessoa = await pessoaRepositorio.ObterPorIdAsync(comando.Id) ?? throw new Exception("Pessoa não encontrada");

            pessoa.Atualizar(
                comando.Nome,
                comando.Sexo,
                comando.Email,
                comando.DataNascimento,
                comando.Naturalidade,
                comando.Nacionalidade
            );

            if (comando.Endereco != null)
            {
                if (pessoa.Endereco != null)
                {
                    pessoa.Endereco.Atualizar(
                        comando.Endereco.Cep,
                        comando.Endereco.Logradouro,
                        comando.Endereco.Numero,
                        comando.Endereco.Complemento,
                        comando.Endereco.Bairro,
                        comando.Endereco.Cidade,
                        comando.Endereco.Estado
                    );
                }
                else
                {
                    var novoEndereco = new Endereco(
                        comando.Endereco.Cep,
                        comando.Endereco.Logradouro,
                        comando.Endereco.Numero,
                        comando.Endereco.Complemento,
                        comando.Endereco.Bairro,
                        comando.Endereco.Cidade,
                        comando.Endereco.Estado
                    );
                    
                    pessoa.AtualizarEndereco(novoEndereco);
                }
            }

            await pessoaRepositorio.AtualizarPessoaComEnderecoAsync(pessoa);

            await servicoCache.RemoverAsync("lista_pessoas");
            await servicoCache.RemoverAsync($"pessoa_{comando.Id}");

            return PessoaFactory.CriarDTO(pessoa);
        }
    }
}
