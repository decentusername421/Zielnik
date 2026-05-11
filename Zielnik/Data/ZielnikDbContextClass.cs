using Microsoft.EntityFrameworkCore;
using Zielnik.Entities;
using Zielnik.Data;

namespace Zielnik.Data
{
    public class ZielnikDbContext : DbContext
    {
        public ZielnikDbContext(
            DbContextOptions<ZielnikDbContext> options)
            : base(options)
        {
        }
        public DbSet<Garden> Gardens { get; set; }
    }
}