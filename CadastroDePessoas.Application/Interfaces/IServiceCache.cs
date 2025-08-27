namespace CadastroDePessoas.Application.Interfaces
{
    public interface IServiceCache
    {
        Task<T> ObterAsync<T>(string chave);
        Task DefinirAsync<T>(string chave, T valor, TimeSpan? tempoExpiracao = null);
        Task RemoverAsync(string chave);
    }
}
