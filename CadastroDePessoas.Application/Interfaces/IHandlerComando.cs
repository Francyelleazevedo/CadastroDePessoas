using MediatR;

namespace CadastroDePessoas.Application.Interfaces
{
    public interface IHandlerComando<TComando, TResposta> : IRequestHandler<TComando, TResposta> where TComando : IRequest<TResposta>
    {
        Task<TResposta> Handle(TComando comando, CancellationToken cancellationToken);
    }
}
