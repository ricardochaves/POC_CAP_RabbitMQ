using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class SystemContext : DbContext
    {
        public SystemContext(DbContextOptions<SystemContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
