using GPS.PropostaService.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GPS.PropostaService.Infrastructure.Configurations
{
    public class PropostaConfiguration : IEntityTypeConfiguration<Proposta>
    {
        public void Configure(EntityTypeBuilder<Proposta> builder)
        {
            builder.ToTable("Propostas");

            builder.HasKey(p => p.IdProposta);
            builder.Property(p => p.IdProposta).ValueGeneratedNever();
            builder.Property(p => p.IdPessoa).IsRequired();
            builder.Property(p => p.Tipo).IsRequired();

            builder.Property(p => p.Status)
                   .HasConversion<string>()
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(p => p.Valor).HasColumnType("decimal(18,2)").IsRequired();

            builder.Property(p => p.DataSolicitacao)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("GETUTCDATE()")
                   .IsRequired();

            builder.Property(p => p.IdUsuarioResponsavel).IsRequired(false);

            builder.HasIndex(p => p.IdPessoa);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => new { p.Tipo, p.Status });
        }
    }
}
