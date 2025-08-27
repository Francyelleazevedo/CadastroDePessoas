using CadastroDePessoas.Domain.Entidades;

namespace CadastroDePessoas.Domain.Interfaces
{
    public interface IPessoaRepositorio : IBaseRepositorio<Pessoa>
    {
        Task<bool> CpfExisteAsync(string cpf, Guid? ignorarId = null);

        Task AtualizarPessoaComEnderecoAsync(Pessoa pessoa);
    }
}
