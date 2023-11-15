using PokerOfflineClient.ViewModels;

namespace PokerOfflineClient.Pages;

public partial class ListRoomsPage : ContentPage
{
    public ListRoomsPage(ListRoomsViewModel vm)
	{
        InitializeComponent();
        BindingContext = vm;
    }
}