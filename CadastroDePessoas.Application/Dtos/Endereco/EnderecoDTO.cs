namespace CadastroDePessoas.Application.Dtos.Endereco
{
    public class EnderecoDTO
    {
        public Guid Id { get; set; }
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }

        public bool EnderecoCompleto => !string.IsNullOrEmpty(Logradouro) &&
                                        !string.IsNullOrEmpty(Numero) &&
                                        !string.IsNullOrEmpty(Bairro) &&
                                        !string.IsNullOrEmpty(Cidade) &&
                                        !string.IsNullOrEmpty(Estado) &&
                                        !string.IsNullOrEmpty(Cep);

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Logradouro) && string.IsNullOrEmpty(Numero))
                return "";

            var enderecoCompleto = $"{Logradouro}, {Numero}";

            if (!string.IsNullOrEmpty(Complemento))
                enderecoCompleto += $", {Complemento}";

            if (!string.IsNullOrEmpty(Bairro))
                enderecoCompleto += $", {Bairro}";

            if (!string.IsNullOrEmpty(Cidade) && !string.IsNullOrEmpty(Estado))
                enderecoCompleto += $", {Cidade} - {Estado}";
            else if (!string.IsNullOrEmpty(Cidade))
                enderecoCompleto += $", {Cidade}";
            else if (!string.IsNullOrEmpty(Estado))
                enderecoCompleto += $", {Estado}";

            if (!string.IsNullOrEmpty(Cep))
                enderecoCompleto += $", {Cep}";

            return enderecoCompleto;
        }
    }
}

