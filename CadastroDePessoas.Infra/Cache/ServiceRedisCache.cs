using CadastroDePessoas.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace CadastroDePessoas.Infra.Cache
{
    public class ServiceRedisCache : IServiceCache
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly JsonSerializerOptions _jsonOptions;

        public ServiceRedisCache(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = redis.GetDatabase();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<T> ObterAsync<T>(string chave)
        {
            var valor = await _database.StringGetAsync(chave);
            if (valor.IsNullOrEmpty) return default;

            return JsonSerializer.Deserialize<T>(valor, _jsonOptions);
        }

        public async Task DefinirAsync<T>(string chave, T valor, TimeSpan? tempoExpiracao = null)
        {
            var valorJson = JsonSerializer.Serialize(valor, _jsonOptions);
            await _database.StringSetAsync(chave, valorJson, tempoExpiracao);
        }

        public async Task RemoverAsync(string chave)
        {
            await _database.KeyDeleteAsync(chave);
        }
    }
}
