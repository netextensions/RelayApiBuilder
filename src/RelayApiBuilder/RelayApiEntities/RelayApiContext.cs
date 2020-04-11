using Microsoft.EntityFrameworkCore;

namespace NetExtensions.RelayApiEntities
{
    public class RelayApiContext : DbContext
    {
        public RelayApiContext()
        {
        }

        public RelayApiContext(DbContextOptions<RelayApiContext> options)
            : base(options)
        {
        }

        public DbSet<ResourceConfigData> ResourceConfigData { get; set; }
    }
}