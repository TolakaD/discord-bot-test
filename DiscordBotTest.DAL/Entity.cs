using System.ComponentModel.DataAnnotations;

namespace DiscordBotTest.DAL
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}
