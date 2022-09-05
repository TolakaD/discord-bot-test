using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordBotTest.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequireCategoriesAttribute : CheckBaseAttribute
    {
        public IReadOnlyList<string> CategoryNames { get; }
        public ChannelCheckmode CheckMode { get; }

        public RequireCategoriesAttribute(ChannelCheckmode checkMode, params string[] categoryNames)
        {
            CheckMode = checkMode;
            CategoryNames = new ReadOnlyCollection<string>(categoryNames);
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            if (ctx.Guild == null || ctx.Member == null)
            {
                return Task.FromResult(false);
            }

            var contains = CategoryNames.Contains(ctx.Channel.Parent.Name, StringComparer.OrdinalIgnoreCase);

            return CheckMode switch
            {
                ChannelCheckmode.Any => Task.FromResult(contains),
                ChannelCheckmode.None=> Task.FromResult(!contains),
                _ => Task.FromResult(false),
            };
        }
    }
}
