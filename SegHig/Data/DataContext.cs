using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SegHig.Data.Entities;

namespace SegHig.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<ClienteTipo> ClienteTipos { get; set; }
        public DbSet<EmpresaTipo> EmpresaTipos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<TrabajoTipo> TrabajoTipos { get; set; }
        public DbSet<Formulario> Formularios { get; set; }
        public DbSet<FormularioDetalle> FormularioDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ClienteTipo>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<EmpresaTipo>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Empresa>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Cliente>().HasIndex("Name", "EmpresaId").IsUnique();
            modelBuilder.Entity<TrabajoTipo>().HasIndex("Name", "ClienteId").IsUnique();
            modelBuilder.Entity<Formulario>().HasIndex("Name", "TrabajoTipoId").IsUnique();
            modelBuilder.Entity<FormularioDetalle>().HasIndex("Description", "FormularioId").IsUnique();
        }
    }
}