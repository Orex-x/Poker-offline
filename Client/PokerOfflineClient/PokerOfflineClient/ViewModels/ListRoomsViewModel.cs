using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PokerOffline.Services.ApiClient;
using PokerOfflineClient.Pages;
using System.Collections.ObjectModel;

namespace PokerOfflineClient.ViewModels
{
    public partial class ListRoomsViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<string> items;


        private readonly IApiClient _apiClient;


        public ListRoomsViewModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
            Task.Run(LoadData);
        }

        public async Task LoadData()
        {
            Items = new(await _apiClient.GetRooms());
        }

        [RelayCommand]
        async Task Tap(string name)
        {
            var ok = await _apiClient.JoinRoom(name);
            if (ok)
                await Shell.Current.GoToAsync($"{nameof(RoomPage)}?RoomName={name}");
        }

        [RelayCommand]
        async Task CreateNewRoom(string name)
        {
            await Task.CompletedTask;
            //await Shell.Current.GoToAsync(nameof(CreateRoomPage));
        }
    }
}
