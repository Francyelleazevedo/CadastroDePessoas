using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.AtualizarPessoa;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.ExcluirPessoa;
using CadastroDePessoas.Application.CQRS.Consultas.ListarPessoas;
using CadastroDePessoas.Application.CQRS.Consultas.ObterPessoa;
using CadastroDePessoas.Application.Dtos.Pessoa;
using CadastroDePessoas.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadastroDePessoas.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class PessoaController(IMediator mediator, ILogger<PessoaController> logger, IServiceCache serviceCache) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PessoaDTO>>> ObterTodos()
        {
            try
            {
                await serviceCache.RemoverAsync("lista_pessoas");
                var resultado = await mediator.Send(new ListarPessoasConsulta());
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao buscar pessoas", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PessoaDTO>> ObterPorId(Guid id)
        {
            try
            {
                await serviceCache.RemoverAsync($"pessoa_{id}");

                var resultado = await mediator.Send(new ObterPessoaConsulta(id));
                if (resultado == null)
                {
                    return NotFound(new { message = $"Pessoa não encontrada" });
                }
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao buscar pessoa", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<PessoaDTO>> Criar(CriarPessoaComando comando)
        {
            try
            {
                var resultado = await mediator.Send(comando);
                await serviceCache.RemoverAsync("lista_pessoas");

                return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao criar pessoa", detail = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult<PessoaDTO>> Atualizar(AtualizarPessoaComando comando)
        {
           try
           {
               var resultado = await mediator.Send(comando);
               await serviceCache.RemoverAsync($"pessoa_{comando.Id}");
               await serviceCache.RemoverAsync("lista_pessoas");
               return Ok(resultado);
           }
           catch (Exception ex)
           {
               return BadRequest(new { message = "Erro ao atualizar pessoa", detail = ex.Message });
           }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Remover(Guid id)
        {
            try
            {
                var resultado = await mediator.Send(new ExcluirPessoaComando(id));
                await serviceCache.RemoverAsync($"pessoa_{id}");
                await serviceCache.RemoverAsync("lista_pessoas");
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao remover pessoa", detail = ex.Message });
            }
        }

        [HttpPost("limpar-cache")]
        [AllowAnonymous]
        public async Task<IActionResult> LimparCache()
        {
            try
            {
                await serviceCache.RemoverAsync("lista_pessoas");
                return Ok(new { message = "Cache limpo com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao limpar cache", detail = ex.Message });
            }
        }
    }
}

