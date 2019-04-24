using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BotGrigory.Core.Handler;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace BotGrigory
{
    public class Program
    {
        public DiscordSocketClient socketClient;
        public CommandService commands;
        IServiceProvider services;


        public string clientId = "561917818132496386";
        public string token = "NTYxOTE3ODE4MTMyNDk2Mzg2.XKDWzQ.kDV8LBnw5EQGJg3knKzVSlKV23M";
        public string[] badWords = new string[1] {"1"};
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public static Program program()
        {
            return null;
        }

        private async Task MainAsync()
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            socketClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,

            });
            commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });
            services = new ServiceCollection().AddSingleton(this).AddSingleton(socketClient).AddSingleton(commands)
                .BuildServiceProvider();
            CensoreHandler blacklistWordsHandle = new CensoreHandler();
            Console.WriteLine(Assembly.GetEntryAssembly().Location);


            socketClient.MessageReceived += CommandHandler;
            await blacklistWordsHandle.BlackListWordsHandler(socketClient);
            await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);


            socketClient.Ready += Client_Ready;
            socketClient.Log += Client_Log;



            await socketClient.LoginAsync(TokenType.Bot, token);
            await socketClient.StartAsync();



            await Task.Delay(-1);





        }


        private async Task Client_Log(LogMessage cltLog)
        {
            Console.WriteLine($"[{DateTime.Now}] [{cltLog.Source}] {cltLog.Message}");
        }

        private async Task Client_Ready()
        {
            Bot_Activity();
        }

        private async Task Bot_Activity()
        {
            await socketClient.SetGameAsync("за каждым участником.", "", ActivityType.Watching);
        }

        private async Task CommandHandler(SocketMessage MsgRcv)
        {
            var message = MsgRcv as SocketUserMessage;
            IGuildUser user = MsgRcv.Author as IGuildUser;
            int ArgPos = 0;
            if (message == null || message.Content == ".")
            {
                return;
            }

            if (!message.HasStringPrefix(".", ref ArgPos))
            {
                return;
            }

            var context = new SocketCommandContext(socketClient, message);
            var result = await commands.ExecuteAsync(context, ArgPos, services);

            if (!result.IsSuccess)
            {
                await message.Channel.SendMessageAsync(result.ErrorReason);
            }

        }
    }
}
