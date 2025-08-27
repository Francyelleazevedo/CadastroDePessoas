using MediatR;

namespace CadastroDePessoas.Application.CQRS.Comandos.Pessoa.ExcluirPessoa
{
    public class ExcluirPessoaComando(Guid id) : IRequest<bool>
    {
        public Guid Id { get; set; } = id;
    }
}
