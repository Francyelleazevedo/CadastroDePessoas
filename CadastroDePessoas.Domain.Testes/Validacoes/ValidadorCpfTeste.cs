using System;
using Xunit;
using CadastroDePessoas.Domain.Validacoes;

namespace CadastroDePessoas.Domain.Testes.Validacoes
{
    public class ValidadorCpfTeste
    {
        [Theory]
        [InlineData("52998224725")]
        [InlineData("529.982.247-25")]
        public void Validar_Deve_Retornar_True_Para_Cpf_Valido(string cpf)
        {
            Assert.True(ValidadorCpf.Validar(cpf));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("11111111111")]
        [InlineData("12345678900")]
        [InlineData("00000000000")]
        [InlineData("5299822472")]
        [InlineData("529982247251")] 
        public void Validar_Deve_Retornar_False_Para_Cpf_Invalido(string cpf)
        {
            Assert.False(ValidadorCpf.Validar(cpf));
        }

        [Fact]
        public void Formatar_Deve_Retornar_Cpf_Formatado()
        {
            var cpf = "52998224725";
            var formatado = ValidadorCpf.Formatar(cpf);
            Assert.Equal("529.982.247-25", formatado);
        }

        [Fact]
        public void Formatar_Deve_Retornar_Entrada_Quando_Cpf_Invalido()
        {
            var cpf = "123";
            var formatado = ValidadorCpf.Formatar(cpf);
            Assert.Equal("123", formatado);
        }
    }
}
