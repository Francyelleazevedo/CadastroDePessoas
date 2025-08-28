using CadastroDePessoas.API.Controllers;
using CadastroDePessoas.Application.CQRS.Comandos.Usuario.AutenticarUsuario;
using CadastroDePessoas.Application.CQRS.Comandos.Usuario.CriarUsuario;
using CadastroDePessoas.Application.Dtos.Usuario;
using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Infra.Contexto;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;

namespace CadastroDePessoas.API.Testes.Controllers
{
    public class AuthControllerTests : IDisposable
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AppDbContexto _dbContext;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            
            var options = new DbContextOptionsBuilder<AppDbContexto>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _dbContext = new AppDbContexto(options);
            
            _controller = new AuthController(_mediatorMock.Object, _dbContext);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        [Fact]
        public async Task Login_ComCredenciaisValidas_DeveRetornarOk()
        {
            // Arrange
            var comando = new AutenticarUsuarioComando
            {
                Email = "maria@exemplo.com",
                Senha = "Senha123"
            };

            var token = "token-jwt-valido";

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ReturnsAsync(token);

            var usuario = new Usuario("Maria José", "maria@exemplo.com", "senha-hash");
            await _dbContext.Usuarios.AddAsync(usuario);
            await _dbContext.SaveChangesAsync();

            // Act
            var resultado = await _controller.Login(comando);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsAssignableFrom<object>(okResult.Value);
            
            // Verificar propriedades dinâmicas
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "success");
            Assert.Contains(properties, p => p.Name == "token");
            Assert.Contains(properties, p => p.Name == "user");

            var successProperty = properties.First(p => p.Name == "success");
            var tokenProperty = properties.First(p => p.Name == "token");
            
            Assert.Equal(true, successProperty.GetValue(response));
            Assert.Equal(token, tokenProperty.GetValue(response));
        }

        [Fact]
        public async Task Login_ComUsuarioNulo_DeveRetornarUnauthorized()
        {
            // Arrange
            var comando = new AutenticarUsuarioComando
            {
                Email = "naoexiste@exemplo.com",
                Senha = "Senha123"
            };

            var token = "token-jwt-valido";

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ReturnsAsync(token);

            // Não adicionar nenhum usuário ao banco (simulando usuário não encontrado)

            // Act
            var resultado = await _controller.Login(comando);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(resultado.Result);
            var response = Assert.IsAssignableFrom<object>(unauthorizedResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "success");
            Assert.Contains(properties, p => p.Name == "message");

            var successProperty = properties.First(p => p.Name == "success");
            Assert.Equal(false, successProperty.GetValue(response));
        }

        [Fact]
        public async Task Login_ComCredenciaisInvalidas_DeveRetornarUnauthorized()
        {
            // Arrange
            var comando = new AutenticarUsuarioComando
            {
                Email = "maria@exemplo.com",
                Senha = "SenhaErrada"
            };

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Credenciais inv�lidas"));

            // Act
            var resultado = await _controller.Login(comando);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(resultado.Result);
            var response = Assert.IsAssignableFrom<object>(unauthorizedResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "success");
            Assert.Contains(properties, p => p.Name == "message");

            var successProperty = properties.First(p => p.Name == "success");
            Assert.Equal(false, successProperty.GetValue(response));
        }

        [Fact]
        public async Task Register_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var comando = new CriarUsuarioComando
            {
                Nome = "Maria José",
                Email = "maria@exemplo.com",
                Senha = "Senha123"
            };

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _controller.Registrar(comando);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsAssignableFrom<object>(okResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "success");
            Assert.Contains(properties, p => p.Name == "message");

            var successProperty = properties.First(p => p.Name == "success");
            Assert.Equal(true, successProperty.GetValue(response));
        }

        [Fact]
        public async Task Register_ComErro_DeveRetornarBadRequest()
        {
            // Arrange
            var comando = new CriarUsuarioComando
            {
                Nome = "Maria José",
                Email = "maria@exemplo.com",
                Senha = "Senha123"
            };

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("E-mail j� existe"));

            // Act
            var resultado = await _controller.Registrar(comando);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            var response = Assert.IsAssignableFrom<object>(badRequestResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "success");
            Assert.Contains(properties, p => p.Name == "message");

            var successProperty = properties.First(p => p.Name == "success");
            Assert.Equal(false, successProperty.GetValue(response));
        }

        [Fact]
        public void VerifyToken_ComTokenValido_DeveRetornarOk()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim("sub", userId),
                new Claim("email", "maria@exemplo.com"),
                new Claim("name", "Maria José")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

            // Act
            var resultado = _controller.VerificarToken();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var response = Assert.IsAssignableFrom<object>(okResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "valid");
            Assert.Contains(properties, p => p.Name == "user");

            var validProperty = properties.First(p => p.Name == "valid");
            Assert.Equal(true, validProperty.GetValue(response));
        }

        [Fact]
        public async Task ChangePassword_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var senhaAtual = BCrypt.Net.BCrypt.HashPassword("senha123", 12);
            var usuario = new Usuario("Maria José", "maria@exemplo.com", senhaAtual);

            await _dbContext.Usuarios.AddAsync(usuario);
            await _dbContext.SaveChangesAsync();

            ConfigurarUsuarioNaRequisicao(usuario.Id.ToString());

            var request = new AlterarSenhaDTO
            {
                SenhaAtual = "senha123",
                NovaSenha = "novaSenha123",
                ConfirmarSenha = "novaSenha123"
            };

            // Act
            var resultado = await _controller.AlterarSenha(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsAssignableFrom<object>(okResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "success");
            Assert.Contains(properties, p => p.Name == "message");

            var successProperty = properties.First(p => p.Name == "success");
            Assert.Equal(true, successProperty.GetValue(response));
        }

        [Fact]
        public async Task ChangePassword_ComSenhaAtualIncorreta_DeveRetornarBadRequest()
        {
            // Arrange
            var senhaAtual = BCrypt.Net.BCrypt.HashPassword("senha123", 12);
            var usuario = new Usuario("Maria José", "maria@exemplo.com", senhaAtual);

            await _dbContext.Usuarios.AddAsync(usuario);
            await _dbContext.SaveChangesAsync();

            ConfigurarUsuarioNaRequisicao(usuario.Id.ToString());

            var request = new AlterarSenhaDTO
            {
                SenhaAtual = "senha_errada",
                NovaSenha = "novaSenha123",
                ConfirmarSenha = "novaSenha123"
            };

            // Act
            var resultado = await _controller.AlterarSenha(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
        }

        [Fact]
        public async Task ChangePassword_ComSenhaFraca_DeveRetornarBadRequest()
        {
            // Arrange
            var senhaAtual = BCrypt.Net.BCrypt.HashPassword("senha123", 12);
            var usuario = new Usuario("Maria José", "maria@exemplo.com", senhaAtual);

            await _dbContext.Usuarios.AddAsync(usuario);
            await _dbContext.SaveChangesAsync();

            ConfigurarUsuarioNaRequisicao(usuario.Id.ToString());

            var request = new AlterarSenhaDTO
            {
                SenhaAtual = "senha123",
                NovaSenha = "123",
                ConfirmarSenha = "123"
            };

            // Act
            var resultado = await _controller.AlterarSenha(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
        }

        [Fact]
        public async Task ChangePassword_ComSenhasNaoCoincidentes_DeveRetornarBadRequest()
        {
            // Arrange
            var senhaAtual = BCrypt.Net.BCrypt.HashPassword("senha123", 12);
            var usuario = new Usuario("Maria José", "maria@exemplo.com", senhaAtual);

            await _dbContext.Usuarios.AddAsync(usuario);
            await _dbContext.SaveChangesAsync();

            ConfigurarUsuarioNaRequisicao(usuario.Id.ToString());

            var request = new AlterarSenhaDTO
            {
                SenhaAtual = "senha123",
                NovaSenha = "novaSenha123",
                ConfirmarSenha = "outraSenha123"
            };

            // Act
            var resultado = await _controller.AlterarSenha(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
        }

        private void ConfigurarUsuarioNaRequisicao(string userId)
        {
            var claims = new List<Claim>
            {
                new Claim("sub", userId),
                new Claim("email", "maria@exemplo.com"),
                new Claim("name", "Maria José")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }
    }
}