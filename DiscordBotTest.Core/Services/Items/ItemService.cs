using System.Threading.Tasks;
using DiscordBotTest.DAL;
using DiscordBotTest.DAL.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace DiscordBotTest.Core.Services.Items
{
    public interface IItemService
    {
        Task<Item> GetItemByName(string itemName);
        Task CreateNewItemAsync(Item item);
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

        public async Task CreateNewItemAsync(Item item)
        {
            await _context.AddAsync(item);
            await _context.SaveChangesAsync();
        }
    }
}
