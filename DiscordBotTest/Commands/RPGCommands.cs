using System.Threading.Tasks;
using DiscordBotTest.Core.Services.Items;
using DiscordBotTest.DAL.Models.Items;
using DiscordBotTest.Handlers.Dialogue;
using DiscordBotTest.Handlers.Dialogue.Steps;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordBotTest.Commands
{
    public class RPGCommands : BaseCommandModule
    {
        private readonly IItemService _itemService;

        public RPGCommands(IItemService itemService)
        {
            _itemService = itemService;
        }

        [Command("createitem")]
        [RequireRoles(RoleCheckMode.Any, "Admin")]
        public async Task CreateItem(CommandContext ctx)
        {
            var itemDescriptionStep = new TextDialogueStep("What is the item description?", null);
            var itemNameStep = new TextDialogueStep("What will the item be called?", itemDescriptionStep);

            var item = new Item();

            itemNameStep.OnValidResult += (result) => item.Name = result;
            itemDescriptionStep.OnValidResult += (result) => item.Description = result;

            var dialogueHandler = new DialogueHandler(ctx.Client, ctx.Channel, ctx.User, itemNameStep);
            bool succeeded = await dialogueHandler.ProcessDialog();
            if (!succeeded) { return; }

            await _itemService.CreateNewItemAsync(item);
            
            await ctx.Channel.SendMessageAsync($"Item with Name: {item.Name} and Description: {item.Description} was successfully created");
        }

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