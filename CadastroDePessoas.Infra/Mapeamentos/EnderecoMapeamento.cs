using CadastroDePessoas.Domain.Entidades;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CadastroDePessoas.Infra.Mapeamentos
{
    public class EnderecoMapeamento : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Cep)
                .HasMaxLength(10);

            builder.Property(e => e.Logradouro)
                .HasMaxLength(200);

            builder.Property(e => e.Numero)
                .HasMaxLength(20);

            builder.Property(e => e.Complemento)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(e => e.Bairro)
                .HasMaxLength(100);

            builder.Property(e => e.Cidade)
                .HasMaxLength(100);

            builder.Property(e => e.Estado)
                .HasMaxLength(2);

            builder.HasOne(e => e.Pessoa)
                .WithOne(p => p.Endereco)
                .HasForeignKey<Endereco>(e => e.PessoaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
