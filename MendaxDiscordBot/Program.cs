using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MendaxDiscordBot
{
    class Program
    {
        DiscordSocketClient _client;
        CommandHandler _handler;

        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        private async Task StartAsync()
        {
            if (Config.bot.botToken == "" || Config.bot.botToken == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            _client.Log += Log;
            //_client.MessageReceived += ReactingToMessages;
            _client.MessageReceived += BadWordsWarn;
            await _client.SetGameAsync("https://mendax.one/mBot.html");
            await _client.SetStatusAsync(UserStatus.DoNotDisturb);
            await _client.LoginAsync(TokenType.Bot, Config.bot.botToken);
            await _client.StartAsync();
            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
            await Task.Delay(-1);
        }

        private async Task BadWordsWarn(SocketMessage message)
        {
            string[] badWords = File.ReadAllLines("bad_words.txt");

            if (badWords.Any(word => message.Content.IndexOf(word, 0, message.Content.Length, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                var m = (RestUserMessage)await message.Channel.GetMessageAsync(message.Id);
                await m.DeleteAsync();
            }
        }

        /*private async Task ReactingToMessages(SocketMessage message)
        {
            if (message.Channel.Id != 450240927810846730) return;
            var ThumbsUp = new Emoji("⏫");
            var ThumbsDown = new Emoji("⏬");
            var m = (RestUserMessage) await message.Channel.GetMessageAsync(message.Id);
            await m.AddReactionAsync(ThumbsUp);
            await m.AddReactionAsync(ThumbsDown);
        }*/

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
        }
    }
}