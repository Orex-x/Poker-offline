using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PokerOffline.Services.ApiClient;
using PokerOfflineClient.Pages;

namespace PokerOfflineClient.ViewModels
{
    public partial class CreateRoomViewModel : ObservableObject
    {
        [ObservableProperty]
        string roomName;

        private readonly IApiClient _apiClient;

        public CreateRoomViewModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [RelayCommand]
        async Task CreateRoom()
        {
            var ok = await _apiClient.CreateRoom(RoomName);
            if (ok)
            {
                ok = await _apiClient.JoinRoom(RoomName);
                if (ok)
                {
                    await Shell.Current.GoToAsync($"{nameof(RoomPage)}?RoomName={RoomName}");
                }
            }
        }
    }
}
