using CadastroDePessoas.Application.Dtos.Pessoa;
using MediatR;

namespace CadastroDePessoas.Application.CQRS.Consultas.ListarPessoas
{
    public class ListarPessoasConsulta : IRequest<IEnumerable<PessoaDTO>>
    {
    }
}
