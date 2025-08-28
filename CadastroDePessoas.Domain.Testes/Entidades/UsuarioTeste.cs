using System;
using Xunit;
using CadastroDePessoas.Domain.Entidades;

namespace CadastroDePessoas.Domain.Testes.Entidades
{
    public class UsuarioTeste
    {
        [Fact]
        public void Deve_Criar_Usuario_Com_Dados_Validos()
        {
            // Arrange & Act
            var usuario = new Usuario("Fulano", "fulano@email.com", "senha123");

            // Assert
            Assert.Equal("Fulano", usuario.Nome);
            Assert.Equal("fulano@email.com", usuario.Email);
            Assert.Equal("senha123", usuario.Senha);
            Assert.NotEqual(Guid.Empty, usuario.Id);
            Assert.True(usuario.DataCadastro <= DateTime.UtcNow);
        }

        [Fact]
        public void Deve_Alterar_Senha()
        {
            // Arrange
            var usuario = new Usuario("Fulano", "fulano@email.com", "senha123");

            // Act
            usuario.AlterarSenha("novaSenha456");

            // Assert
            Assert.Equal("novaSenha456", usuario.Senha);
        }
    }
}
