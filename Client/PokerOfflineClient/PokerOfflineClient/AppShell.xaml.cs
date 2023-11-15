using PokerOfflineClient.Pages;

namespace PokerOfflineClient;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(RoomPage), typeof(RoomPage));
        Routing.RegisterRoute(nameof(ListRoomsPage), typeof(ListRoomsPage));
    }
}
