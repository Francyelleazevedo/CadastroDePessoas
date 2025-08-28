using CadastroDePessoas.Application.CQRS.Comandos.Usuario.AutenticarUsuario;
using CadastroDePessoas.Application.CQRS.Comandos.Usuario.CriarUsuario;
using CadastroDePessoas.Application.Dtos.Usuario;
using CadastroDePessoas.Infra.Contexto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BC = BCrypt.Net.BCrypt;

namespace CadastroDePessoas.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController(IMediator mediator, AppDbContexto dbContext) : ControllerBase
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Login([FromBody] AutenticarUsuarioComando loginDto)
        {
            try
            {
                var token = await mediator.Send(loginDto);

                var usuario = await dbContext.Usuarios
                    .Where(u => u.Email == loginDto.Email)
                    .Select(u => new
                    {
                        u.Id,
                        u.Nome,
                        u.Email
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Usuário não encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Login realizado com sucesso",
                    token,
                    user = new
                    {
                        id = usuario.Id,
                        nome = usuario.Nome,
                        email = usuario.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("registrar")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Registrar([FromBody] CriarUsuarioComando criarUsuarioDto)
        {
            try
            {
                var sucesso = await mediator.Send(criarUsuarioDto);

                return Ok(new
                {
                    success = sucesso,
                    message = "Usuário criado com sucesso",
                    user = new
                    {
                        name = criarUsuarioDto.Nome,
                        email = criarUsuarioDto.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("perfil")]
        [Authorize]
        public async Task<ActionResult<object>> ObterPerfil()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new { message = "Token inválido" });
            }

            try
            {
                var usuario = await dbContext.Usuarios
                    .Where(u => u.Id == userGuid)
                    .Select(u => new
                    {
                        u.Id,
                        u.Nome,
                        u.Email,
                        DataCadastro = u.DataCadastro
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                return Ok(new
                {
                    success = true,
                    user = usuario
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno do servidor: {ex.Message}" });
            }
        }

        [HttpPut("alterar-senha")]
        [Authorize]
        public async Task<ActionResult<object>> AlterarSenha([FromBody] AlterarSenhaDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new { message = "Token inválido" });
            }

            try
            {
                var usuario = await dbContext.Usuarios.FindAsync(userGuid);

                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                if (!BC.Verify(dto.SenhaAtual, usuario.Senha))
                {
                    return BadRequest(new { message = "Senha atual incorreta" });
                }

                if (dto.NovaSenha.Length < 8)
                {
                    return BadRequest(new { message = "A nova senha deve ter pelo menos 8 caracteres" });
                }

                if (dto.NovaSenha != dto.ConfirmarSenha)
                {
                    return BadRequest(new { message = "A nova senha e a confirmação não coincidem" });
                }

                usuario.AlterarSenha(BC.HashPassword(dto.NovaSenha, 12));

                await dbContext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Senha alterada com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno do servidor: {ex.Message}" });
            }
        }

        [HttpGet("verificar-token")]
        [Authorize]
        public ActionResult VerificarToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value ?? User.FindFirst("name")?.Value;

            return Ok(new
            {
                valid = true,
                user = new
                {
                    id = userId,
                    email,
                    name
                }
            });
        }
    }
}