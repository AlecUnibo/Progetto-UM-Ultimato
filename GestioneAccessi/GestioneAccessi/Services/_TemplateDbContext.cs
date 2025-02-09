using GestioneAccessi.Infrastructure;
using GestioneAccessi.Services.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestioneAccessi.Services
{
    public class GestioneAccessiDbContext : DbContext
    {
        public GestioneAccessiDbContext()
        {
        }

        public GestioneAccessiDbContext(DbContextOptions<GestioneAccessiDbContext> options) : base(options)
        {
            DataGenerator.InitializeUsers(this);
        }

        public DbSet<User> Users { get; set; }
    }
}
