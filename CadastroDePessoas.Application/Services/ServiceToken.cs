using CadastroDePessoas.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CadastroDePessoas.Application.Services
{
    public class ServiceToken : IServiceToken
    {
        private readonly IConfiguration _configuration;

        public ServiceToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string ObterChaveJwt()
        {
            // Prioriza variável de ambiente, depois appsettings
            var chave = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? _configuration["Jwt:Chave"];

            if (string.IsNullOrEmpty(chave))
            {
                throw new InvalidOperationException("Chave JWT não configurada. Configure a variável de ambiente JWT_SECRET_KEY ou a configuração Jwt:Chave.");
            }

            return chave;
        }

        public string GerarToken(Guid usuarioId, string email, string nome, IEnumerable<string> roles = null)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuarioId.ToString()),
                new(JwtRegisteredClaimNames.Email, email),
                new(JwtRegisteredClaimNames.Name, nome),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var chaveSecreta = ObterChaveJwt();
            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
            var expiracao = DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpiracionHoras"] ?? "1"));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Emissor"],
                audience: _configuration["Jwt:Audiencia"],
                claims: claims,
                expires: expiracao,
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidarToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("O token não pode ser nulo ou vazio", nameof(token));
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                // Verificar se o token tem o formato JWT válido
                if (!tokenHandler.CanReadToken(token))
                {
                    throw new ArgumentException("O token fornecido não está no formato JWT válido", nameof(token));
                }

                var chaveSecreta = ObterChaveJwt();
                var chave = Encoding.UTF8.GetBytes(chaveSecreta);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(chave),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Emissor"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audiencia"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                return principal;
            }
            catch (SecurityTokenMalformedException)
            {
                throw new ArgumentException("O token fornecido não está no formato JWT válido", nameof(token));
            }
            catch (Exception ex) when (
                ex is SecurityTokenInvalidSignatureException ||
                ex is SecurityTokenInvalidIssuerException ||
                ex is SecurityTokenInvalidAudienceException ||
                ex is SecurityTokenExpiredException)
            {
                throw new ArgumentException($"Token inválido: {ex.Message}", nameof(token));
            }
        }
    }
}
