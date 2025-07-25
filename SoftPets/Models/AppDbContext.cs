using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;


namespace SoftPets.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("ConexionLocal") { }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Duenio> Duenios { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Vacuna> Vacunas { get; set; }
        public DbSet<Vacunacion> Vacunaciones { get; set; }
        public DbSet<HistorialClinico> HistorialesClinicos { get; set; }
        public DbSet<Tendencia> Tendencias { get; set; }
        public DbSet<Veterinario> Veterinarios { get; set; }
        public DbSet<QRPublico> QRPublicos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Clave compuesta en QRPublico
            modelBuilder.Entity<QRPublico>  ()
                .HasKey(q => new { q.MascotaId, q.Campo });

            // Relaciones 1:1 entre Usuario y Duenio
            modelBuilder.Entity<Usuario>()
                .HasOptional(u => u.Duenio)
                .WithRequired(d => d.Usuario);

            // Relaciones 1:1 entre Usuario y Veterinario
            modelBuilder.Entity<Usuario>()
                .HasOptional(u => u.Veterinario)
                .WithRequired(v => v.Usuario);

            // Relaciones 1:N para Mascotas y Duenios
            modelBuilder.Entity<Duenio>()
                .HasMany(d => d.Mascotas)
                .WithRequired(m => m.Duenio)
                .HasForeignKey(m => m.DuenioId);

            // Relaciones para Mascota con Vacunacion, HistorialClinico, Tendencia, QRPublico
            modelBuilder.Entity<Mascota>()
                .HasMany(m => m.Vacunaciones)
                .WithRequired(v => v.Mascota)
                .HasForeignKey(v => v.MascotaId);

            modelBuilder.Entity<Mascota>()
                .HasMany(m => m.HistorialesClinicos)
                .WithRequired(h => h.Mascota)
                .HasForeignKey(h => h.MascotaId);

            modelBuilder.Entity<Mascota>()
                .HasMany(m => m.Tendencias)
                .WithRequired(t => t.Mascota)
                .HasForeignKey(t => t.MascotaId);

            modelBuilder.Entity<Mascota>()
                .HasMany(m => m.QRPublicos)
                .WithRequired(q => q.Mascota)
                .HasForeignKey(q => q.MascotaId);

            // Relaciones para Vacuna con Vacunacion
            modelBuilder.Entity<Vacuna>()
                .HasMany(v => v.Vacunaciones)
                .WithRequired(vac => vac.Vacuna)
                .HasForeignKey(vac => vac.VacunaId);

            // Relaciones para Usuario (como veterinario) con Vacunacion e HistorialClinico
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.VacunacionesRealizadas)
                .WithRequired(v => v.Veterinario)
                .HasForeignKey(v => v.VeterinarioId);

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.HistorialesClinicosRealizados)
                .WithRequired(h => h.Veterinario)
                .HasForeignKey(h => h.VeterinarioId);

            base.OnModelCreating(modelBuilder);
        }
    }
}