using CadastroDePessoas.Application.Dtos.Pessoa;
using CadastroDePessoas.Application.Factories;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Interfaces;

namespace CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa
{
    public class CriarPessoaComandoHandler(IPessoaRepositorio pessoaRepositorio, IServiceCache servicoCache) : IHandlerComando<CriarPessoaComando, PessoaDTO>
    {
        public async Task<PessoaDTO> Handle(CriarPessoaComando comando, CancellationToken cancellationToken)
        {
            if (await pessoaRepositorio.CpfExisteAsync(comando.Cpf))
                throw new System.Exception("CPF já está em uso.");

            var pessoaDto = comando.ParaDTO();
            var pessoa = PessoaFactory.CriarEntidade(pessoaDto);

            await pessoaRepositorio.AdicionarAsync(pessoa);

            await servicoCache.RemoverAsync("lista_pessoas");

            return PessoaFactory.CriarDTO(pessoa);
        }
    }
}
