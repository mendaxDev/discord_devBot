using Discord.Commands;
using System.Threading.Tasks;
using MendaxDiscordBot.Core.UserProfiles;
using Discord;
using Discord.WebSocket;
using System;
using System.Linq;

namespace MendaxDiscordBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {

        [Command("ban")]
        public async Task BanUser(IGuildUser user, [Remainder] string reason)
        {
            await Context.Message.DeleteAsync();

            if (string.IsNullOrWhiteSpace(reason)) return;

            var allBans = await Context.Guild.GetBansAsync();
            bool isBanned = allBans.Select(b => b.User).Where(u => u.Username == user.Username).Any();

            if (!isBanned)
            {
                var targetHighest = (user as SocketGuildUser).Hierarchy;
                var senderHighest = (Context.User as SocketGuildUser).Hierarchy;

                if (targetHighest < senderHighest)
                {
                    await Context.Guild.AddBanAsync(user);

                    EmbedBuilder ban = new EmbedBuilder();

                    ban.WithTitle($"Ban: " + DateTime.Now)
                        .WithDescription($"Banned User: {user.Username}\nBanned From: {Context.User.Username}\nFor Reason: {reason}")
                        .WithColor(Color.DarkRed);

                    await Context.Guild.GetTextChannel(451342695139508235).SendMessageAsync("", false, ban.Build());

                }
            }
        }

        /// <summary>
        /// $Warn Command
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Command("warn")]
        public async Task WarnUser(SocketUser user)
        {
            var accounts = UserAccounts.GetAccount(user);
            var allBans = await Context.Guild.GetBansAsync();
            bool isBanned = allBans.Select(b => b.User).Where(u => u.Username == user.Username).Any();

            if (accounts.Warns <= 1)
            {
                accounts.Warns++;
                UserAccounts.SaveAccounts();

                await ReplyAsync($"I've warned {user.Username}, he/she got already warned {accounts.Warns}x and will in {3 - accounts.Warns} more warns.");
            }
            else if (accounts.Warns >= 2 && !isBanned)
            {
                    var targetHighest = (user as SocketGuildUser).Hierarchy;
                    var senderHighest = (Context.User as SocketGuildUser).Hierarchy;

                    if (targetHighest < senderHighest)
                    {
                        accounts.Warns++;
                        await Context.Guild.AddBanAsync(user);

                        EmbedBuilder ban = new EmbedBuilder();

                        ban.WithTitle($"Ban: " + DateTime.Now)
                            .WithDescription($"Banned User: {user.Username}\nBanned From: {Context.Client.CurrentUser.Username}\nFor Reason: 3 warns")
                            .WithColor(Color.DarkRed);

                        await Context.Channel.SendMessageAsync("", embed: ban.Build());

                    }
            }
        }

        /// <summary>
        /// $Stats Command
        /// </summary>
        /// <returns></returns>
        [Command("stats")]
        public async Task MyEXP()
        {
            var embed = new EmbedBuilder();
           
            var account = UserAccounts.GetAccount(Context.User);
            embed
                .WithTitle("Stats form " + Context.Message.Author.Username)
                .WithDescription($"Level: {account.Level}\nEXP: {account.EXP}");

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        /// <summary>
        /// $AddEXP Command
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        [Command("addEXP")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddEXP(uint exp)
        {
            var accounts = UserAccounts.GetAccount(Context.User);
            accounts.EXP += exp;
            UserAccounts.SaveAccounts();
            await ReplyAsync($"You gained {exp} EXP!");
        }
    }
}
