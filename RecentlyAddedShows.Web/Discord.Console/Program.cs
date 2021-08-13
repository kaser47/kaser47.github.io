using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Discord.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new DiscordSocketConfig()
            {

            };
            var discord = new DiscordSocketClient();


            await discord.LoginAsync(TokenType.Bot, 
                "ODc1NzM3OTcxNDQwMDk1MzMy.YRZ4ig.vlMkzGMVG-IQPt_z0uQOjvwrjbE", 
                true);
            await discord.StartAsync();

            while (discord.Guilds.All(x => x.Name != null))
            { }

            var channels = new List<SocketTextChannel>();

            var i = 0;
            foreach (var guild in discord.Guilds)
            {
                while (guild.Channels.Count == 0)
                { }

                channels.AddRange((guild.Channels.Where(x => x.GetType() == typeof(SocketTextChannel)).Cast<SocketTextChannel>().ToList()));
                i++;
            }
            
            var j = 0;
            foreach (var channel in channels)
            {
                System.Console.WriteLine($"{channel.Guild.Name} - {j} - {channel.Name}");
                j++;
            }

            System.Console.WriteLine("Select a channel");
            int channelNo = 0;
            int.TryParse(System.Console.ReadLine(), out channelNo);

            var selectedChannel = channels.ToArray()[channelNo];
            var messagechannel = selectedChannel as IMessageChannel;

            System.Console.WriteLine("Enter your messages");

            string text = string.Empty;
            while(text != "EXIT")
            {
                text = System.Console.ReadLine();
                if (text != "EXIT")
                {
                    messagechannel.SendMessageAsync(text);
                }
            }
        }
    }
}
