using System.Threading.Tasks;
using DiscordBotTest.Core.Services.Items;
using DiscordBotTest.DAL;
using DiscordBotTest.DAL.Models.Items;
using DiscordBotTest.Handlers.Dialogue;
using DiscordBotTest.Handlers.Dialogue.Steps;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DiscordBotTest.Commands
{
    public class RPGCommands : BaseCommandModule
    {
        private readonly IItemService _itemService;

        public RPGCommands(IItemService itemService)
        {
            _itemService = itemService;
        }

        //[Command("additem")]
        //public async Task AddItem(CommandContext ctx, string name)
        //{
        //    await _itemService.Items.AddAsync(new Item { Name = name, Description = "Test Description" });
        //    await _itemService.SaveChangesAsync();

        //    var result = await _itemService.Items.ToListAsync();
        //}

        [Command("iteminfo")]
        public async Task ItemInfo(CommandContext ctx)
        {
            var itemNameStep = new TextDialogueStep("What item are you looking for?", null);
            string itemName = string.Empty;
            itemNameStep.OnValidResult += (result) => itemName = result;

            var dialogueHandler = new DialogueHandler(ctx.Client, ctx.Channel, ctx.User, itemNameStep);
            bool succeeded = await dialogueHandler.ProcessDialog();
            if (!succeeded) { return; }

            var item = await _itemService.GetItemByName(itemName);

            if (item == null)
            {
                await ctx.Channel.SendMessageAsync($"There is no item called '{itemName}'");
                return;
            }

            await ctx.Channel.SendMessageAsync($"Name: {item.Name}, Description: {item.Description}");
        }
    }
}