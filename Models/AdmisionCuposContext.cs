using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProyectoSC_AE.Models
{
    public partial class AdmisionCuposContext : DbContext
    {
        public AdmisionCuposContext()
        {
        }

        public AdmisionCuposContext(DbContextOptions<AdmisionCuposContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Asignatura> Asignaturas { get; set; } = null!;
        public virtual DbSet<EstudianteNuevosArchivo> EstudianteNuevosArchivos { get; set; } = null!;
        public virtual DbSet<EstudiantesNuevo> EstudiantesNuevos { get; set; } = null!;
        public virtual DbSet<EstudiantesSeccione> EstudiantesSecciones { get; set; } = null!;
        public virtual DbSet<Pensum> Pensums { get; set; } = null!;
        public virtual DbSet<PosiblesCupo> PosiblesCupos { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asignatura>(entity =>
            {
                entity.Property(e => e.Asignatura1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Asignatura");

                entity.Property(e => e.Codigo)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Creditos)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.IdPensum).HasColumnName("Id_Pensum");

                entity.HasOne(d => d.IdPensumNavigation)
                    .WithMany(p => p.Asignaturas)
                    .HasForeignKey(d => d.IdPensum)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Asignaturas_Pensums");
            });

            modelBuilder.Entity<EstudianteNuevosArchivo>(entity =>
            {
                entity.Property(e => e.EmailEstudiante)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Estatus)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.FechaCreacion).HasColumnType("datetime");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tipo)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EstudiantesNuevo>(entity =>
            {
                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Estatus)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Tipo)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EstudiantesSeccione>(entity =>
            {
                entity.ToTable("Estudiantes_Secciones");

                entity.Property(e => e.Estatus)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.IdEstudiante).HasColumnName("Id_Estudiante");

                entity.Property(e => e.IdPosiblesCupos).HasColumnName("Id_PosiblesCupos");

                entity.HasOne(d => d.IdEstudianteNavigation)
                    .WithMany(p => p.EstudiantesSecciones)
                    .HasForeignKey(d => d.IdEstudiante)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Estudiantes_Secciones_Usuarios");

                entity.HasOne(d => d.IdPosiblesCuposNavigation)
                    .WithMany(p => p.EstudiantesSecciones)
                    .HasForeignKey(d => d.IdPosiblesCupos)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Estudiantes_Secciones_PosiblesCupos");
            });

            modelBuilder.Entity<Pensum>(entity =>
            {
                entity.Property(e => e.Codigo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Estatus)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Pensum1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Pensum");
            });

            modelBuilder.Entity<PosiblesCupo>(entity =>
            {
                entity.Property(e => e.Asignatura)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Codigo)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Creditos)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.Estatus)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Horario)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Estatus)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Matricula)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tipo)
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
