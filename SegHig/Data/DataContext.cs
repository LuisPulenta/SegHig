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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ClienteTipo>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<EmpresaTipo>().HasIndex(c => c.Name).IsUnique();
        }
    }
}