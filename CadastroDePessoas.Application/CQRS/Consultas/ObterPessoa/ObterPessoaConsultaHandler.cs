using CadastroDePessoas.Application.Dtos.Pessoa;
using CadastroDePessoas.Application.Factories;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Interfaces;

namespace CadastroDePessoas.Application.CQRS.Consultas.ObterPessoa
{
    public class ObterPessoaConsultaHandler(IPessoaRepositorio pessoaRepositorio, IServiceCache servicoCache) : IHandlerConsulta<ObterPessoaConsulta, PessoaDTO>
    {
        private readonly IPessoaRepositorio _pessoaRepositorio = pessoaRepositorio ?? throw new ArgumentNullException(nameof(pessoaRepositorio));
        private readonly IServiceCache _servicoCache = servicoCache ?? throw new ArgumentNullException(nameof(servicoCache));

        public async Task<PessoaDTO> Handle(ObterPessoaConsulta consulta, CancellationToken cancellationToken)
        {
            var cacheKey = $"pessoa_{consulta.Id}";
            var pessoaCache = await _servicoCache.ObterAsync<PessoaDTO>(cacheKey);
            if (pessoaCache != null)
                return pessoaCache;

            var pessoa = await _pessoaRepositorio.ObterPorIdAsync(consulta.Id) ?? throw new Exception("Pessoa não encontrada");
            var pessoaDto = PessoaFactory.CriarDTO(pessoa);

            await _servicoCache.DefinirAsync(cacheKey, pessoaDto, TimeSpan.FromMinutes(5));

            return pessoaDto;
        }
    }
}
