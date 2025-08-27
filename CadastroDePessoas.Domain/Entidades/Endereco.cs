namespace CadastroDePessoas.Domain.Entidades
{
    public class Endereco
    {
        public Guid Id { get; private set; }
        public string Cep { get; private set; }
        public string Logradouro { get; private set; }
        public string Numero { get; private set; }
        public string Complemento { get; private set; }
        public string Bairro { get; private set; }
        public string Cidade { get; private set; }
        public string Estado { get; private set; }
        public Guid PessoaId { get; private set; }
        public Pessoa Pessoa { get; private set; }

        public Endereco(string cep, string logradouro, string numero, string complemento, string bairro, string cidade, string estado)
        {
            Id = Guid.NewGuid();
            Cep = cep;
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
        }

        protected Endereco() { }

        public void Atualizar(string cep, string logradouro, string numero, string complemento, string bairro, string cidade, string estado)
        {
            Cep = cep;
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
        }

        public void DefinirPessoaId(Guid pessoaId)
        {
            if (PessoaId == Guid.Empty)
            {
                PessoaId = pessoaId;
            }
        }

        public override string ToString()
        {
            var enderecoCompleto = $"{Logradouro}, {Numero}";

            if (!string.IsNullOrEmpty(Complemento))
                enderecoCompleto += $", {Complemento}";

            enderecoCompleto += $", {Bairro}, {Cidade} - {Estado}, {Cep}";

            return enderecoCompleto;
        }
    }
}
