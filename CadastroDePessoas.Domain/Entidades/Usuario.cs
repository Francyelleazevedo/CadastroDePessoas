namespace CadastroDePessoas.Domain.Entidades
{
    public class Usuario
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Senha { get; private set; }
        public DateTime DataCadastro { get; private set; }

        public Usuario(string nome, string email, string senha)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Email = email;
            Senha = senha;
            DataCadastro = DateTime.UtcNow;
        }

        protected Usuario() { }

        public void AlterarSenha(string novaSenha)
        {
            Senha = novaSenha;
        }
    }
}
