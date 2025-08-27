using CadastroDePessoas.Application.Dtos.Pessoa;
using CadastroDePessoas.Application.Factories;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Interfaces;

namespace CadastroDePessoas.Application.CQRS.Consultas.ListarPessoas
{
    public class ListarPessoasConsultaHandler(IPessoaRepositorio pessoaRepositorio, IServiceCache servicoCache) : IHandlerConsulta<ListarPessoasConsulta, IEnumerable<PessoaDTO>>
    {

        public async Task<IEnumerable<PessoaDTO>> Handle(ListarPessoasConsulta consulta, CancellationToken cancellationToken)
        {
            var pessoasCache = await servicoCache.ObterAsync<IEnumerable<PessoaDTO>>("lista_pessoas");
            if (pessoasCache != null) return pessoasCache;

            var pessoas = await pessoaRepositorio.ListarAsync();
            var pessoasDto = pessoas.Select(PessoaFactory.CriarDTO).ToList();

            await servicoCache.DefinirAsync("lista_pessoas", pessoasDto, TimeSpan.FromMinutes(5));

            return pessoasDto;
        }
    }
}
