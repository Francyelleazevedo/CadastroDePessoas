using CadastroDePessoas.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CadastroDePessoas.Application.Testes.Services
{
    public class ServiceTokenTeste
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IConfigurationSection> _jwtSectionMock;
        private readonly ServiceToken _serviceToken;
        private readonly string _chaveSecreta;
        private readonly string _emissor;
        private readonly string _audiencia;
        private readonly string _expiracaoHoras;

        public ServiceTokenTeste()
        {
            _chaveSecreta = "s3cr3t_k3y_para_t3st3s_JWT_token_dev_@2025";
            _emissor = "TestEmissor";
            _audiencia = "TestAudiencia";
            _expiracaoHoras = "1";

            _configurationMock = new Mock<IConfiguration>();
            _jwtSectionMock = new Mock<IConfigurationSection>();

            _configurationMock.Setup(c => c["Jwt:Chave"]).Returns(_chaveSecreta);
            _configurationMock.Setup(c => c["Jwt:Emissor"]).Returns(_emissor);
            _configurationMock.Setup(c => c["Jwt:Audiencia"]).Returns(_audiencia);
            _configurationMock.Setup(c => c["Jwt:ExpiracionHoras"]).Returns(_expiracaoHoras);

            _serviceToken = new ServiceToken(_configurationMock.Object);
        }

        [Fact]
        public void GerarToken_ComDadosValidos_DeveRetornarTokenJWT()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var email = "teste@teste.com";
            var nome = "Usuário Teste";
            var roles = new List<string> { "Admin", "User" };

            // Act
            var token = _serviceToken.GerarToken(usuarioId, email, nome, roles);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            // Verificar se o token é um JWT válido
            var tokenHandler = new JwtSecurityTokenHandler();
            Assert.True(tokenHandler.CanReadToken(token));

            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Verificar claims
            Assert.Equal(usuarioId.ToString(), jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
            Assert.Equal(email, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
            Assert.Equal(nome, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value);
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");

            // Verificar emissor e audiência
            Assert.Equal(_emissor, jwtToken.Issuer);
            Assert.Equal(_audiencia, jwtToken.Audiences.FirstOrDefault());
        }

        [Fact]
        public void GerarToken_SemRoles_DeveGerarTokenSemClaimsDeRole()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var email = "teste@teste.com";
            var nome = "Usuário Teste";

            // Act
            var token = _serviceToken.GerarToken(usuarioId, email, nome);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.Equal(usuarioId.ToString(), jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
            Assert.Equal(email, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
            Assert.Equal(nome, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value);
            Assert.DoesNotContain(jwtToken.Claims, c => c.Type == ClaimTypes.Role);
        }

        [Fact]
        public void ValidarToken_ComTokenValido_DeveRetornarClaimsPrincipal()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var email = "teste@teste.com";
            var nome = "Usuário Teste";
            var roles = new List<string> { "Admin" };

            var token = _serviceToken.GerarToken(usuarioId, email, nome, roles);

            // Act
            var principal = _serviceToken.ValidarToken(token);

            // Assert
            Assert.NotNull(principal);

            // Verificar apenas se o principal tem identity e claims
            Assert.NotNull(principal.Identity);
            Assert.True(principal.Claims.Any());
        }

        [Fact]
        public void ObterChaveJwt_QuandoChaveEstaConfigurada_DeveRetornarChave()
        {
            // Act & Assert
            var usuarioId = Guid.NewGuid();
            var token = _serviceToken.GerarToken(usuarioId, "email@teste.com", "Nome");

            Assert.NotNull(token);
            var principal = _serviceToken.ValidarToken(token);
            Assert.NotNull(principal);
        }

        [Fact]
        public void ValidarToken_ComTokenInvalido_DeveLancarArgumentException()
        {
            // Arrange
            var tokenInvalido = "tokeninvalido";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _serviceToken.ValidarToken(tokenInvalido));
        }
    }
}