using System.Threading.Tasks;

namespace Codenames.Shared
{
    public interface IGameHub
    {
        Task AddToGroupAsync(string groupId);

        Task RemoveFromGroupAsync(string groupId);

        Task UpdateGameAsync(string gameId, string updatedGame);

        Task UpdatePlayerIdentificationAsync(string gameId, string playerName, bool identified);

        Task SendGameMessageAsync(string chatId, string eventName, GameMessage chatMessage);

        Task AddNewPlayerToGameAsync(string gameId, Player player);
    }
}