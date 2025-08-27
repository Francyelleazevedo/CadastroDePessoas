using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CadastroDePessoas.API.Filtros
{
    public class ApiExcecaoFiltro : ExceptionFilterAttribute
    {
        private readonly ILogger<ApiExcecaoFiltro> _logger;
        private readonly Dictionary<Type, Action<ExceptionContext>> _manipuladoresExcecao;

        public ApiExcecaoFiltro(ILogger<ApiExcecaoFiltro> logger)
        {
            _logger = logger;
            _manipuladoresExcecao = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(ValidationException), ManipularValidacaoExcecao },
                { typeof(UnauthorizedAccessException), ManipularNaoAutorizadoExcecao },
                { typeof(Exception), ManipularExcecaoGeral }
            };
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);

            var tipo = context.Exception.GetType();
            if (_manipuladoresExcecao.ContainsKey(tipo))
            {
                _manipuladoresExcecao[tipo].Invoke(context);
                return;
            }

            ManipularExcecaoDesconhecida(context);
            base.OnException(context);
        }

        private void ManipularValidacaoExcecao(ExceptionContext context)
        {
            var excecao = context.Exception as ValidationException;
            var detalhes = new ValidationProblemDetails
            {
                Title = "Ocorreram um ou mais erros de validação",
                Status = StatusCodes.Status400BadRequest,
                Detail = excecao.Message,
                Instance = context.HttpContext.Request.Path
            };

            foreach (var erro in excecao.Errors)
            {
                detalhes.Errors.Add(erro.PropertyName, new[] { erro.ErrorMessage });
            }

            context.Result = new BadRequestObjectResult(detalhes);
            context.ExceptionHandled = true;
        }

        private void ManipularNaoAutorizadoExcecao(ExceptionContext context)
        {
            var detalhes = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Não autorizado",
                Detail = context.Exception.Message,
                Instance = context.HttpContext.Request.Path
            };

            context.Result = new UnauthorizedObjectResult(detalhes);
            context.ExceptionHandled = true;
        }

        private void ManipularExcecaoGeral(ExceptionContext context)
        {
            var detalhes = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Ocorreu um erro ao processar sua solicitação",
                Detail = context.Exception.Message,
                Instance = context.HttpContext.Request.Path
            };

            context.Result = new ObjectResult(detalhes)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }

        private void ManipularExcecaoDesconhecida(ExceptionContext context)
        {
            var detalhes = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Ocorreu um erro ao processar sua solicitação",
                Detail = context.Exception.Message,
                Instance = context.HttpContext.Request.Path
            };

            context.Result = new ObjectResult(detalhes)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}
