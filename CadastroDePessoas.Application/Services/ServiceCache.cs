using CadastroDePessoas.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace CadastroDePessoas.Application.Services
{
    public class ServicoCache : IServiceCache
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _jsonOptions;

        public ServicoCache(IDistributedCache cache)
        {
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<T> ObterAsync<T>(string chave)
        {
            if (string.IsNullOrEmpty(chave))
                throw new ArgumentException("A chave não pode ser nula ou vazia", nameof(chave));

            try
            {
                var bytesValorCache = await _cache.GetAsync(chave);
                if (bytesValorCache == null || bytesValorCache.Length == 0)
                    return default;

                var valorCache = Encoding.UTF8.GetString(bytesValorCache);
                return JsonSerializer.Deserialize<T>(valorCache, _jsonOptions);
            }
            catch (JsonException)
            {
                // Erro na deserialização
                return default;
            }
        }

        public async Task DefinirAsync<T>(string chave, T valor, TimeSpan? tempoExpiracao = null)
        {
            if (string.IsNullOrEmpty(chave))
                throw new ArgumentException("A chave não pode ser nula ou vazia", nameof(chave));

            if (valor == null)
                throw new ArgumentNullException(nameof(valor), "O valor não pode ser nulo");

            var valorJson = JsonSerializer.Serialize(valor, _jsonOptions);
            var bytesValorJson = Encoding.UTF8.GetBytes(valorJson);

            var opcoes = new DistributedCacheEntryOptions();

            if (tempoExpiracao.HasValue)
                opcoes.AbsoluteExpirationRelativeToNow = tempoExpiracao.Value;
            else
                opcoes.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            await _cache.SetAsync(chave, bytesValorJson, opcoes);
        }

        public async Task RemoverAsync(string chave)
        {
            if (string.IsNullOrEmpty(chave))
                throw new ArgumentException("A chave não pode ser nula ou vazia", nameof(chave));

            await _cache.RemoveAsync(chave);
        }
    }
}
