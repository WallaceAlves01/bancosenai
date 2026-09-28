using BancoSENAIAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace BancoSENAIAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Agencia> Agencia => Set<Agencia>();
        public DbSet<Carteira> Carteira => Set<Carteira>();
        public DbSet<Cliente> Cliente => Set<Cliente>();
        public DbSet<DocumentoMetadado> DocumentoMetadados => Set<DocumentoMetadado>();
    }
}
