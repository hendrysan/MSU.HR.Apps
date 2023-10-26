using Discord;
using Discord.WebSocket;
using Repositories.Interfaces;

namespace Repositories.Implements
{
    public class DiscordRepository : IDiscordRepository
    {
        public async Task<bool> SendText(string text)
        {
            try
            {
                var config = new DiscordSocketConfig();

                DiscordSocketClient discord = new DiscordSocketClient();

                ulong serverid = 1166924358439141376;

                //https://discord.com/channels/1166924358439141376/1166924460637552761

                ulong guild = 1166924358439141376;

                ulong chanel = 1166924460637552761;

                var getGuid = discord.GetGuild(guild);
                var getChanel = getGuid.GetTextChannel(chanel);
                var send = getChanel.SendMessageAsync(text);

           

                //discord.GetGuild(guild).GetTextChannel(chanel).SendMessageAsync(text);

                //var chnl = discord.GetChannel(1068346493452423212) as IMessageChannel;
                //await chnl.SendMessageAsync("Announcement!");

                //var test = discord
                //      .GetGuild(1068346493012017274)
                //      .GetTextChannel(1068346493452423212)
                //      .SendMessageAsync(text);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
