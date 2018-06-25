using Discord;
using Discord.WebSocket;
using MendaxDiscordBot.Core.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MendaxDiscordBot.Core.LevelSystem
{
    internal static class Leveling
    {
        internal static async void UserSentMessage(SocketGuildUser user, SocketTextChannel channel)
        {
            var userAccount = UserAccounts.GetAccount(user);

            if (userAccount.NextMessageReward >= DateTime.Now) return;

            else
            {

                uint oldLevel = userAccount.Level;
                userAccount.EXP += 75;
                userAccount.NextMessageReward = DateTime.Now.AddMinutes(5);
                UserAccounts.SaveAccounts();
                uint newLevel = userAccount.Level;

                if (oldLevel != newLevel)
                {
                    var embed = new EmbedBuilder();
                    embed
                        .WithColor(0, 255, 20)
                        .WithTitle("Level UP! :-)")
                        .WithDescription($"{user.Username} just leveled up!")
                        .AddInlineField("Level: ", newLevel)
                        .AddInlineField("EXP: ", userAccount.EXP);

                    await channel.SendMessageAsync("", embed: embed);
                }
            }
        }
    }
}
