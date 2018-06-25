using Discord.Commands;
using Discord.WebSocket;
using MendaxDiscordBot.Core.LevelSystem;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MendaxDiscordBot
{
    internal class CommandHandler
    {
        DiscordSocketClient _client;
        CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(_client, msg);
            if (context.User.IsBot) return;

            Leveling.UserSentMessage((SocketGuildUser)context.User, (SocketTextChannel)context.Channel);

            int argPos = 0;

            if (msg.HasStringPrefix(Config.bot.cmdPrefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}