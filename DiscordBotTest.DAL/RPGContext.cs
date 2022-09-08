using DiscordBotTest.DAL.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace DiscordBotTest.DAL
{
    public class RPGContext : DbContext
    {
        public RPGContext(DbContextOptions<RPGContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
    }
}
