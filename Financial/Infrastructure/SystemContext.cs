using Microsoft.EntityFrameworkCore;

namespace Financial.Infrastructure
{
    public class SystemContext : DbContext
    {
        public SystemContext(DbContextOptions<SystemContext> options) : base(options)
        {
        }
    }
}
