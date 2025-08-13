using GPS.PessoaService.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GPS.PessoaService.Infrastructure.Configurations
{
    public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
    {
        public void Configure(EntityTypeBuilder<Pessoa> builder)
        {
            builder.ToTable("Pessoas");
            builder.HasKey(p => p.IdPessoa);

            builder.Property(p => p.Nome).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Email).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Telefone).HasMaxLength(20).IsRequired();
            builder.Property(p => p.DataNascimento).IsRequired();
        }
    }
}
