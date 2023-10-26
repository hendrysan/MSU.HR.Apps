namespace Repositories.Interfaces
{
    public interface IDiscordRepository
    {
        Task<bool> SendText(string text);
    }
}
