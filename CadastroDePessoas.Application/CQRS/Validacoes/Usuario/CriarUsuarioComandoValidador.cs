using CadastroDePessoas.Application.CQRS.Comandos.Usuario.CriarUsuario;
using FluentValidation;

namespace CadastroDePessoas.Application.CQRS.Validacoes.Usuario
{
    public class CriarUsuarioComandoValidator : AbstractValidator<CriarUsuarioComando>
    {
        public CriarUsuarioComandoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("O nome é obrigatório")
                .MinimumLength(2)
                .WithMessage("O nome deve ter pelo menos 2 caracteres")
                .MaximumLength(100)
                .WithMessage("O nome deve ter no máximo 100 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("O e-mail é obrigatório")
                .EmailAddress()
                .WithMessage("E-mail inválido")
                .MaximumLength(100)
                .WithMessage("O e-mail deve ter no máximo 100 caracteres");

            RuleFor(x => x.Senha)
                .NotEmpty()
                .WithMessage("A senha é obrigatória")
                .MinimumLength(6)
                .WithMessage("A senha deve ter pelo menos 6 caracteres")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
                .WithMessage("A senha deve conter pelo menos: 1 letra minúscula, 1 maiúscula, 1 número e 1 caractere especial");
        }
    }
}