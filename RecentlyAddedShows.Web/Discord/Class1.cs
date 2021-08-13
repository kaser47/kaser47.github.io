using Discord.WebSocket;
using System;
using Discord.Rest;

namespace Discord
{
    public class Class1
    {
        public async void Run()
        {
            var config = new DiscordSocketConfig()
            {

            };
            var discord = new DiscordSocketClient();

            
            await discord.LoginAsync(TokenType.Bot, "ODc1NzM3OTcxNDQwMDk1MzMy.YRZ4ig.vlMkzGMVG-IQPt_z0uQOjvwrjbE", true);
            var task2 = discord.StartAsync();


            var result = discord.DMChannels;
        }
    }
}
