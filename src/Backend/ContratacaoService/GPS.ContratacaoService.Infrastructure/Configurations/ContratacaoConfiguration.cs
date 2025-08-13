using GPS.ContratacaoService.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GPS.ContratacaoService.Infrastructure.Configurations
{
    public class ContratacaoConfiguration : IEntityTypeConfiguration<Contratacao>
    {
        public void Configure(EntityTypeBuilder<Contratacao> builder)
        {
            builder.ToTable("Contratacoes");

            builder.HasKey(c => c.IdContratacao);

            builder.Property(c => c.IdContratacao).ValueGeneratedNever();

            builder.Property(c => c.IdProposta).IsRequired();

            builder.Property(c => c.DataContratacao)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("GETUTCDATE()")
                   .IsRequired();

            builder.HasIndex(c => c.IdProposta);
        }
    }
}
