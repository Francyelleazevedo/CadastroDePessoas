namespace CadastroDePessoas.Application.Dtos.Usuario
{
    public class UsuarioDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataCadastro { get; set; }

        public TimeSpan TempoDesdeRegistro
        {
            get
            {
                var tempoDecorrido = DateTime.Now - DataCadastro;
                return tempoDecorrido < TimeSpan.Zero ? TimeSpan.Zero : tempoDecorrido;
            }
        }

        public bool EstaRegistradoHaMaisDeUmMes
        {
            get
            {
                return (DateTime.Now - DataCadastro).TotalDays > 30;
            }
        }

        public UsuarioDTO() { }

        public UsuarioDTO(Guid id, string nome, string email, DateTime dataCadastro)
        {
            Id = id;
            Nome = nome;
            Email = email;
            DataCadastro = dataCadastro;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Nome: {Nome}, Email: {Email}, DataCadastro: {DataCadastro:dd/MM/yyyy}";
        }
    }
}
