using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PokerOffline.Services.ApiClient;

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
        string textButtonNextTap;

        [ObservableProperty]
        bool enableButtonNextTap;

        [ObservableProperty]
        ImageSource firstCard;

        [ObservableProperty]
        ImageSource secondCard;


        [ObservableProperty]
        ImageSource table1, table2, table3, table4, table5;

        private string[] actions = new[] { "/startGame", "/flop",  "/turn", "/river" };
        private int actionIndex = 0;


        public RoomViewModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
            TextButtonNextTap = "StartGame";
            gameEvent = "Waiting";
            EnableButtonNextTap = true;

            Task.Run(Start);       
        }

        public async Task Start()
        {
            await Task.Delay(1000); 
            while (true)
            {
                await StatusHandler();
            }
        }


        [RelayCommand]
        async Task TapNextStep()
        {
            await _apiClient.DoAction(RoomName, actions[actionIndex]);
            actionIndex++;
            if (actionIndex == actions.Length)
                actionIndex = 0;
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
                        FirstCard = ImageSource.FromFile($"{cards[0]}.jpg");
                        SecondCard = ImageSource.FromFile($"{cards[1]}.jpg");
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
                        FirstCard = ImageSource.FromFile($"{cards[0]}.jpg");
                        SecondCard = ImageSource.FromFile($"{cards[1]}.jpg");
                    }
                    break;
                case "FinishGame":
                    {
                        Table1 = null;
                        Table2 = null;
                        Table3 = null;
                        Table4 = null;
                        Table5 = null;
                        FirstCard = null;
                        SecondCard = null;
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
                        Table1 = ImageSource.FromFile($"{cards[0]}.jpg");
                        Table2 = ImageSource.FromFile($"{cards[1]}.jpg");
                        Table3 = ImageSource.FromFile($"{cards[2]}.jpg");

                    }
                    break;
                case "Turn":
                    {
                        TextButtonNextTap  = "River";
                        var table = await _apiClient.GetTable(RoomName);
                        var cards = table.Split(' ');
                        Table4 = ImageSource.FromFile($"{cards[3]}.jpg");
                    }
                    break;
                case "River":
                    {
                        TextButtonNextTap = "";
                        EnableButtonNextTap = false;

                        var table = await _apiClient.GetTable(RoomName);
                        var cards = table.Split(' ');
                        Table5 = ImageSource.FromFile($"{cards[4]}.jpg");
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
