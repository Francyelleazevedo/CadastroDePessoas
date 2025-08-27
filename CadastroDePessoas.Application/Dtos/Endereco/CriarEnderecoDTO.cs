using CadastroDePessoas.Domain.Enums;

namespace CadastroDePessoas.Application.Dtos.Endereco
{
    public class CriarEnderecoDTO
    {
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
    }
}
