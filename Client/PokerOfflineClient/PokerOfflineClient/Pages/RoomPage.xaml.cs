using PokerOfflineClient.ViewModels;

namespace PokerOfflineClient.Pages;

public partial class RoomPage : ContentPage
{
    private readonly RoomViewModel _roomViewModel;
    public RoomPage(RoomViewModel vm)
	{
        var tapGestureRecognizer = new TapGestureRecognizer();

        InitializeComponent();
        _roomViewModel = vm;
        BindingContext = vm;
    }


    protected override bool OnBackButtonPressed()
    {
        Task<bool> task = Task.Run(_roomViewModel.ClosePage);
        var ok = task.Result;
        // false - close | true - not close
        var backButtonPressed = base.OnBackButtonPressed(); 
        return !ok || backButtonPressed;
    }
}