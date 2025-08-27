using CadastroDePessoas.Application.Dtos.Pessoa;
using MediatR;

namespace CadastroDePessoas.Application.CQRS.Consultas.ObterPessoa
{
    public class ObterPessoaConsulta(Guid id) : IRequest<PessoaDTO>
    {
        public Guid Id { get; set; } = id;
    }
}
