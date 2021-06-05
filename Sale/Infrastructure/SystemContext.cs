using Microsoft.EntityFrameworkCore;
using Sale.Controllers;

namespace Sale.Infrastructure
{
    public class SystemContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public SystemContext(DbContextOptions<SystemContext> options) : base(options)
        {
        }
    }
}
