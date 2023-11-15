namespace PokerOffline.Services.ApiClient
{
    public interface IApiClient
    {
        Task<ICollection<string>> GetRooms();

        Task<bool> JoinRoom(string roomName);

        Task<bool> ExitRoom(string roomName);

        Task<string> GetStatus(string roomName);

        Task<bool> CreateRoom(string roomName);

        Task<bool> StartGame(string roomName);

        Task<bool> RestartGame(string roomName);

        Task<bool> FinishGame(string roomName);

        Task<string> GetHand(string roomName);

        Task<string> GetTable(string roomName);

        Task<bool> Flop(string roomName);

        Task<bool> Turn(string roomName);

        Task<bool> River(string roomName);

        Task<bool> DoAction(string roomName, string action);

        Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content);

        Task<HttpResponseMessage> GetAsync(string endpoint, Dictionary<string, object> queryParams = null);
    }
}
