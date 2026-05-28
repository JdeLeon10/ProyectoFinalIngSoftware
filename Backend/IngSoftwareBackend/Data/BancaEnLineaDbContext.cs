using System;
using System.Collections.Generic;
using IngSoftwareBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace IngSoftwareBackend.Data;

public partial class BancaEnLineaDbContext : DbContext
{
    public BancaEnLineaDbContext()
    {
    }

    public BancaEnLineaDbContext(DbContextOptions<BancaEnLineaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Beneficiario> Beneficiarios { get; set; }

    public virtual DbSet<Cuenta> Cuenta { get; set; }

    public virtual DbSet<CuotaPrestamo> CuotaPrestamos { get; set; }

    public virtual DbSet<MovimientoCuentum> MovimientoCuenta { get; set; }

    public virtual DbSet<PagoPrestamo> PagoPrestamos { get; set; }

    public virtual DbSet<Prestamo> Prestamos { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<SolicitudPrestamo> SolicitudPrestamos { get; set; }

    public virtual DbSet<Transferencium> Transferencia { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Beneficiario>(entity =>
        {
            entity.HasKey(e => e.IdBeneficiario);

            entity.ToTable("beneficiario");

            entity.HasIndex(e => new { e.IdUsuarioOrigen, e.IdCuentaDestino }, "UQ_beneficiario_usuario_cuenta").IsUnique();

            entity.Property(e => e.IdBeneficiario).HasColumnName("id_beneficiario");
            entity.Property(e => e.Alias)
                .HasMaxLength(100)
                .HasColumnName("alias");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Activo")
                .HasColumnName("estado");
            entity.Property(e => e.FechaAgregado)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fecha_agregado");
            entity.Property(e => e.IdCuentaDestino).HasColumnName("id_cuenta_destino");
            entity.Property(e => e.IdUsuarioOrigen).HasColumnName("id_usuario_origen");

            entity.HasOne(d => d.IdCuentaDestinoNavigation).WithMany(p => p.Beneficiarios)
                .HasForeignKey(d => d.IdCuentaDestino)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_beneficiario_cuenta_destino");

            entity.HasOne(d => d.IdUsuarioOrigenNavigation).WithMany(p => p.Beneficiarios)
                .HasForeignKey(d => d.IdUsuarioOrigen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_beneficiario_usuario_origen");
        });

        modelBuilder.Entity<Cuenta>(entity =>
        {
            entity.HasKey(e => e.IdCuenta);

            entity.ToTable("cuenta");

            entity.HasIndex(e => e.NumeroCuenta, "UQ_cuenta_numero").IsUnique();

            entity.Property(e => e.IdCuenta).HasColumnName("id_cuenta");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Activa")
                .HasColumnName("estado");
            entity.Property(e => e.FechaApertura)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fecha_apertura");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NumeroCuenta)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("numero_cuenta");
            entity.Property(e => e.SaldoActual)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_actual");
            entity.Property(e => e.TipoCuenta)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipo_cuenta");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Cuenta)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cuenta_usuario");
        });

        modelBuilder.Entity<CuotaPrestamo>(entity =>
        {
            entity.HasKey(e => e.IdCuota);

            entity.ToTable("cuota_prestamo");

            entity.HasIndex(e => new { e.IdPrestamo, e.NumeroCuota }, "UQ_cuota_prestamo_numero").IsUnique();

            entity.Property(e => e.IdCuota).HasColumnName("id_cuota");
            entity.Property(e => e.CapitalPagado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("capital_pagado");
            entity.Property(e => e.CapitalProgramado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("capital_programado");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente")
                .HasColumnName("estado");
            entity.Property(e => e.FechaVencimiento).HasColumnName("fecha_vencimiento");
            entity.Property(e => e.IdPrestamo).HasColumnName("id_prestamo");
            entity.Property(e => e.InteresPagado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("interes_pagado");
            entity.Property(e => e.InteresProgramado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("interes_programado");
            entity.Property(e => e.MontoTotalProgramado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_total_programado");
            entity.Property(e => e.MoraPagada)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mora_pagada");
            entity.Property(e => e.MoraProgramada)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mora_programada");
            entity.Property(e => e.NumeroCuota).HasColumnName("numero_cuota");

            entity.HasOne(d => d.IdPrestamoNavigation).WithMany(p => p.CuotaPrestamos)
                .HasForeignKey(d => d.IdPrestamo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cuota_prestamo");
        });

        modelBuilder.Entity<MovimientoCuentum>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento);

            entity.ToTable("movimiento_cuenta");

            entity.Property(e => e.IdMovimiento).HasColumnName("id_movimiento");
            entity.Property(e => e.Categoria)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("categoria");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaMovimiento)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fecha_movimiento");
            entity.Property(e => e.IdCuenta).HasColumnName("id_cuenta");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.ReferenciaId).HasColumnName("referencia_id");
            entity.Property(e => e.ReferenciaTipo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("referencia_tipo");
            entity.Property(e => e.SaldoAnterior)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_anterior");
            entity.Property(e => e.SaldoPosterior)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_posterior");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipo_movimiento");

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.MovimientoCuenta)
                .HasForeignKey(d => d.IdCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_movimiento_cuenta");
        });

        modelBuilder.Entity<PagoPrestamo>(entity =>
        {
            entity.HasKey(e => e.IdPagoPrestamo);

            entity.ToTable("pago_prestamo");

            entity.Property(e => e.IdPagoPrestamo).HasColumnName("id_pago_prestamo");
            entity.Property(e => e.CapitalAbonado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("capital_abonado");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Aplicado")
                .HasColumnName("estado");
            entity.Property(e => e.FechaPago)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fecha_pago");
            entity.Property(e => e.IdCuentaOrigen).HasColumnName("id_cuenta_origen");
            entity.Property(e => e.IdCuota).HasColumnName("id_cuota");
            entity.Property(e => e.IdPrestamo).HasColumnName("id_prestamo");
            entity.Property(e => e.InteresAbonado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("interes_abonado");
            entity.Property(e => e.MontoPagado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pagado");
            entity.Property(e => e.MoraAbonada)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("mora_abonada");
            entity.Property(e => e.RegistradoPor).HasColumnName("registrado_por");

            entity.HasOne(d => d.IdCuentaOrigenNavigation).WithMany(p => p.PagoPrestamos)
                .HasForeignKey(d => d.IdCuentaOrigen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pago_cuenta_origen");

            entity.HasOne(d => d.IdCuotaNavigation).WithMany(p => p.PagoPrestamos)
                .HasForeignKey(d => d.IdCuota)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pago_cuota");

            entity.HasOne(d => d.IdPrestamoNavigation).WithMany(p => p.PagoPrestamos)
                .HasForeignKey(d => d.IdPrestamo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pago_prestamo");

            entity.HasOne(d => d.RegistradoPorNavigation).WithMany(p => p.PagoPrestamos)
                .HasForeignKey(d => d.RegistradoPor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pago_registrado_por");
        });

        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.HasKey(e => e.IdPrestamo);

            entity.ToTable("prestamo");

            entity.HasIndex(e => e.IdSolicitudPrestamo, "UQ_prestamo_solicitud").IsUnique();

            entity.Property(e => e.IdPrestamo).HasColumnName("id_prestamo");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Aprobado")
                .HasColumnName("estado");
            entity.Property(e => e.FechaDesembolso)
                .HasPrecision(0)
                .HasColumnName("fecha_desembolso");
            entity.Property(e => e.IdSolicitudPrestamo).HasColumnName("id_solicitud_prestamo");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.MontoAprobado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_aprobado");
            entity.Property(e => e.PlazoMeses).HasColumnName("plazo_meses");
            entity.Property(e => e.SaldoPendiente)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_pendiente");
            entity.Property(e => e.TasaInteres)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("tasa_interes");

            entity.HasOne(d => d.IdSolicitudPrestamoNavigation).WithOne(p => p.Prestamo)
                .HasForeignKey<Prestamo>(d => d.IdSolicitudPrestamo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_prestamo_solicitud");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_prestamo_usuario");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol);

            entity.ToTable("rol");

            entity.HasIndex(e => e.Nombre, "UQ_rol_nombre").IsUnique();

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<SolicitudPrestamo>(entity =>
        {
            entity.HasKey(e => e.IdSolicitudPrestamo);

            entity.ToTable("solicitud_prestamo");

            entity.Property(e => e.IdSolicitudPrestamo).HasColumnName("id_solicitud_prestamo");
            entity.Property(e => e.AprobadoPor).HasColumnName("aprobado_por");
            entity.Property(e => e.DestinoPrestamo)
                .HasMaxLength(200)
                .HasColumnName("destino_prestamo");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente")
                .HasColumnName("estado");
            entity.Property(e => e.FechaResolucion)
                .HasPrecision(0)
                .HasColumnName("fecha_resolucion");
            entity.Property(e => e.FechaSolicitud)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fecha_solicitud");
            entity.Property(e => e.IdCuentaDesembolso).HasColumnName("id_cuenta_desembolso");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.MontoSolicitado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_solicitado");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(500)
                .HasColumnName("observaciones");
            entity.Property(e => e.PlazoMeses).HasColumnName("plazo_meses");

            entity.HasOne(d => d.AprobadoPorNavigation).WithMany(p => p.SolicitudPrestamoAprobadoPorNavigations)
                .HasForeignKey(d => d.AprobadoPor)
                .HasConstraintName("FK_solicitud_aprobado_por");

            entity.HasOne(d => d.IdCuentaDesembolsoNavigation).WithMany(p => p.SolicitudPrestamos)
                .HasForeignKey(d => d.IdCuentaDesembolso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_solicitud_cuenta_desembolso");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.SolicitudPrestamoIdUsuarioNavigations)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_solicitud_usuario");
        });

        modelBuilder.Entity<Transferencium>(entity =>
        {
            entity.HasKey(e => e.IdTransferencia);

            entity.ToTable("transferencia");

            entity.HasIndex(e => e.Referencia, "UQ_transferencia_referencia").IsUnique();

            entity.Property(e => e.IdTransferencia).HasColumnName("id_transferencia");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Completada")
                .HasColumnName("estado");
            entity.Property(e => e.FechaTransferencia)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fecha_transferencia");
            entity.Property(e => e.IdCuentaDestino).HasColumnName("id_cuenta_destino");
            entity.Property(e => e.IdCuentaOrigen).HasColumnName("id_cuenta_origen");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.Referencia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("referencia");

            entity.HasOne(d => d.IdCuentaDestinoNavigation).WithMany(p => p.TransferenciumIdCuentaDestinoNavigations)
                .HasForeignKey(d => d.IdCuentaDestino)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_transferencia_cuenta_destino");

            entity.HasOne(d => d.IdCuentaOrigenNavigation).WithMany(p => p.TransferenciumIdCuentaOrigenNavigations)
                .HasForeignKey(d => d.IdCuentaOrigen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_transferencia_cuenta_origen");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.ToTable("usuario");

            entity.HasIndex(e => e.Dpi, "UQ_usuario_dpi").IsUnique();

            entity.HasIndex(e => e.Email, "UQ_usuario_email").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .HasColumnName("apellidos");
            entity.Property(e => e.Direccion)
                .HasMaxLength(250)
                .HasColumnName("direccion");
            entity.Property(e => e.Dpi)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("dpi");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Activo")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .HasColumnName("nombres");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_usuario_rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
