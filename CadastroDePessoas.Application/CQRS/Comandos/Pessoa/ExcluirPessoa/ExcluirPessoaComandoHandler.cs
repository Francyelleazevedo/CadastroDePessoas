using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Interfaces;

namespace CadastroDePessoas.Application.CQRS.Comandos.Pessoa.ExcluirPessoa
{
    public class ExcluirPessoaComandoHandler(IPessoaRepositorio repositorioPessoa, IServiceCache servicoCache) : IHandlerComando<ExcluirPessoaComando, bool>
    {
        public async Task<bool> Handle(ExcluirPessoaComando comando, CancellationToken cancellationToken)
        {
            var pessoa = await repositorioPessoa.ObterPorIdAsync(comando.Id) ?? throw new Exception("Pessoa não encontrada");

            await repositorioPessoa.RemoverAsync(pessoa);

            await servicoCache.RemoverAsync("lista_pessoas");
            await servicoCache.RemoverAsync($"pessoa_{comando.Id}");

            return true;
        }
    }
}
