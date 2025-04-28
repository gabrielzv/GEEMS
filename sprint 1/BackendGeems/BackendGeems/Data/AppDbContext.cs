using Microsoft.EntityFrameworkCore;
using BackendGeems.Models;
namespace BackendGeems.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
