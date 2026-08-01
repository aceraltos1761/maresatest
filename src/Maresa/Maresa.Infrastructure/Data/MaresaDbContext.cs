using Maresa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Maresa.Infrastructure.Data;

public class MaresaDbContext : DbContext
{
    public MaresaDbContext(DbContextOptions<MaresaDbContext> options) : base(options)
    {
    }

    public DbSet<PedidoCabecera> Pedidos => Set<PedidoCabecera>();
    public DbSet<PedidoDetalle> PedidoDetalles => Set<PedidoDetalle>();
    public DbSet<LogAuditoria> LogsAuditoria => Set<LogAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PedidoCabecera>(entity =>
        {
            entity.ToTable("PedidosCabecera");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Total).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Usuario).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Estado).HasConversion<string>().HasMaxLength(20);

            entity.HasMany(p => p.Detalles)
                .WithOne(d => d.Pedido)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PedidoDetalle>(entity =>
        {
            entity.ToTable("PedidoDetalles");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Precio).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<LogAuditoria>(entity =>
        {
            entity.ToTable("LogsAuditoria");
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Evento).HasMaxLength(100).IsRequired();
            entity.Property(l => l.Descripcion).HasMaxLength(500).IsRequired();

            entity.HasOne(l => l.Pedido)
                .WithMany()
                .HasForeignKey(l => l.PedidoId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
