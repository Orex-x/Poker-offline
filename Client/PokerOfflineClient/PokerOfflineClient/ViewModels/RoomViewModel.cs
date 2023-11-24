using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PokerOffline.Services.ApiClient;
using PokerOfflinelient.ViewModels;
using System.Windows.Input;

namespace PokerOfflineClient.ViewModels
{
    [QueryProperty("RoomName", "RoomName")]
    public partial class RoomViewModel : ObservableObject
    {
        private readonly IApiClient _apiClient;

        private string gameEvent;

        [ObservableProperty]
        string roomName;

        [ObservableProperty]
        string textButtonNextTap = "StartGame";

        [ObservableProperty]
        int countPeople;

        [ObservableProperty]
        bool enableButtonNextTap = true;

        [ObservableProperty]
        private CardViewModel firstCard, secondCard;

        [ObservableProperty]
        ImageSource table1, table2, table3, table4, table5;

        [ObservableProperty]
        ImageSource backgroundImage;

        private string[] actions = new[] { "/startGame", "/flop",  "/turn", "/river" };
        private int actionIndex = 0;


        public RoomViewModel(IApiClient apiClient)
        {
            _apiClient = apiClient;

           
            
            Task.Run(Init);       
        }

        
        public async Task Init()
        {
            var choosenShirt = await SecureStorage.GetAsync("choosen_shirt");

            if (string.IsNullOrEmpty(choosenShirt))
            {
                choosenShirt = "shirt1.png";
            }

            FirstCard = new CardViewModel()
            {
                ShirtFileName = choosenShirt
            };
            SecondCard = new CardViewModel()
            {
                ShirtFileName = choosenShirt
            };

            TapHandCommand = new Command((obj) => {
                var card = obj as CardViewModel;
                card.TurnOver();
            });

            await Task.Delay(1000);
            CountPeople = await _apiClient.GetCountPeople(RoomName);
            while (true)
            {
                await StatusHandler();
            }
        }


        [RelayCommand]
        async Task TapNextStep()
        {
            var ok = await _apiClient.DoAction(RoomName, actions[actionIndex]);
            if (ok)
            {
                actionIndex++;
                if (actionIndex == actions.Length)
                    actionIndex = 0;
            }
        }

        [RelayCommand]
        async Task TapRestartGame()
        {
            await _apiClient.RestartGame(RoomName);
        }

        [RelayCommand]
        async Task TapFinishGame()
        {
            await _apiClient.FinishGame(RoomName);
        }


        [ObservableProperty]
        public ICommand tapHandCommand;

        public async Task StatusHandler()
        {
            gameEvent = await _apiClient.GetStatus(RoomName);

            switch (gameEvent)
            {
                case "StartGame":
                    {
                        TextButtonNextTap  = "Flop";
                        EnableButtonNextTap = true;
                        var hand = await _apiClient.GetHand(RoomName);
                        var cards = hand.Split(' ');

                        FirstCard.CardFileName = $"{cards[0]}.png";
                        SecondCard.CardFileName = $"{cards[1]}.png";

                        FirstCard.ShowShirt();
                        SecondCard.ShowShirt();
                        break;
                    }
                case "RestartGame":
                    {
                        Table1 = null;
                        Table2 = null;
                        Table3 = null;
                        Table4 = null;
                        Table5 = null;
                        actionIndex = 1;
                        EnableButtonNextTap = true;
                        TextButtonNextTap = "Flop";
                        var hand = await _apiClient.GetHand(RoomName);
                        var cards = hand.Split(' ');

                        FirstCard.CardFileName = $"{cards[0]}.png";
                        SecondCard.CardFileName = $"{cards[1]}.png";

                        FirstCard.ShowShirt();
                        SecondCard.ShowShirt();
                    }
                    break;
                case "FinishGame":
                    {
                        Table1 = null;
                        Table2 = null;
                        Table3 = null;
                        Table4 = null;
                        Table5 = null;
                        FirstCard.Source = null;
                        SecondCard.Source = null;
                        actionIndex = 0;
                        TextButtonNextTap  = "Start game";
                        EnableButtonNextTap = true;
                    }
                    break;
                case "Flop":
                    {
                        TextButtonNextTap  = "Turn";
                        var table = await _apiClient.GetTable(RoomName);
                        var cards = table.Split(' ');
                        Table1 = ImageSource.FromFile($"{cards[0]}.png");
                        Table2 = ImageSource.FromFile($"{cards[1]}.png");
                        Table3 = ImageSource.FromFile($"{cards[2]}.png");

                    }
                    break;
                case "Turn":
                    {
                        TextButtonNextTap  = "River";
                        var table = await _apiClient.GetTable(RoomName);
                        var cards = table.Split(' ');
                        Table4 = ImageSource.FromFile($"{cards[3]}.png");
                    }
                    break;
                case "River":
                    {
                        TextButtonNextTap = "";
                        EnableButtonNextTap = false;

                        var table = await _apiClient.GetTable(RoomName);
                        var cards = table.Split(' ');
                        Table5 = ImageSource.FromFile($"{cards[4]}.png");
                    }
                    break;
                case "Join":
                    {
                        CountPeople = await _apiClient.GetCountPeople(RoomName);
                    }
                    break;
                case "Exit":
                    {
                        CountPeople = await _apiClient.GetCountPeople(RoomName);
                    }
                    break;
            }
        }

        public async Task<bool> ClosePage()
        {
            return await _apiClient.ExitRoom(RoomName);
        }
    }
}
