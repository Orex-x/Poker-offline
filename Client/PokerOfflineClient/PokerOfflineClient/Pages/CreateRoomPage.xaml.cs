using PokerOfflineClient.ViewModels;

namespace PokerOfflineClient.Pages;

public partial class CreateRoomPage : ContentPage
{
	public CreateRoomPage(CreateRoomViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}