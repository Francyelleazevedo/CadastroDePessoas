using CadastroDePessoas.Application.CQRS.Comandos.Usuario.AutenticarUsuario;
using FluentValidation;

namespace CadastroDePessoas.Application.CQRS.Validacoes.Usuario
{
    public class AutenticarUsuarioComandoValidador : AbstractValidator<AutenticarUsuarioComando>
    {
        public AutenticarUsuarioComandoValidador()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório")
                .EmailAddress().WithMessage("O e-mail informado não é válido");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("A senha é obrigatória")
                .MinimumLength(6).WithMessage("A senha deve ter no mínimo 8 caracteres");
        }
    }
}
