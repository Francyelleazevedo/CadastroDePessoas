using MediatR;

namespace CadastroDePessoas.Application.Interfaces
{
    public interface IHandlerConsulta<TConsulta, TResposta> : IRequestHandler<TConsulta, TResposta> where TConsulta : IRequest<TResposta>
    {
        Task<TResposta> Handle(TConsulta consulta, CancellationToken cancellationToken);
    }
}
