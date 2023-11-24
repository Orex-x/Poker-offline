using CommunityToolkit.Mvvm.ComponentModel;
using PokerOffline.Services.ApiClient;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PokerOfflineClient.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<ShirtViewModel> shirts;


        [ObservableProperty]
        public ICommand tapCommand;
        
        [ObservableProperty]
        public ICommand saveBaseUrlCommand;

        [ObservableProperty]
        string baseUrl;

        private readonly IApiClient _apiClient;

        public SettingsViewModel(IApiClient apiClient) 
        {
            _apiClient = apiClient;
            Task.Run(Init);
        }

        public async Task Init()
        {
            BaseUrl = await SecureStorage.GetAsync("base_url");
            
            SaveBaseUrlCommand = new Command(async () => {
                await SecureStorage.Default.SetAsync("base_url", BaseUrl);
                _apiClient.SetBaseUrl(BaseUrl);
            });

            TapCommand = new Command(async (obj) => {
                var model = obj as ShirtViewModel;
                await ResetBorder();
                model.BorderSize = 4;
                await SecureStorage.Default.SetAsync("choosen_shirt", model.SourseFile);
            });

            var choosenShirt = await SecureStorage.GetAsync("choosen_shirt");

            if (string.IsNullOrEmpty(choosenShirt))
                choosenShirt = "";

            var list = new List<ShirtViewModel>();
            for (int i = 1; i < 22; i++)
            {

                var sourseFile = $"shirt{i}.png";
                list.Add(new ShirtViewModel()
                {
                    BorderSize = sourseFile == choosenShirt ? 4 : 0,
                    Source = ImageSource.FromFile(sourseFile),
                    SourseFile = sourseFile
                });
            }

            Shirts = new(list);
        }

        async Task ResetBorder()
        {
            await Task.Run(() => {
                foreach (var model in Shirts)
                    model.BorderSize = 0;
            });
        }
    }
}
