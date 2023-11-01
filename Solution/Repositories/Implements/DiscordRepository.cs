using Discord;
using Discord.WebSocket;
using Repositories.Interfaces;

namespace Repositories.Implements
{
    public class DiscordRepository : IDiscordRepository
    {
        public Task<bool> SendText(string text)
        {
            try
            {
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                throw new NullReferenceException(e.Message, e.InnerException);
            }
        }
    }
}
