using System.Linq;
using System.Threading.Tasks;
using DiscordBotTest.DAL;
using DiscordBotTest.DAL.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace DiscordBotTest.Core.Services.Items
{
    public interface IItemService
    {
        Task<Item> GetItemByName(string itemName);
    }

    public class ItemService : IItemService
    {
        private readonly RPGContext _context;

        public ItemService(RPGContext context)
        {
            _context = context;
        }

        public async Task<Item> GetItemByName(string itemName)
        {
            itemName = itemName.ToLower();
            return await _context.Items.FirstOrDefaultAsync(i => i.Name.ToLower() == itemName);
        }
    }
}
