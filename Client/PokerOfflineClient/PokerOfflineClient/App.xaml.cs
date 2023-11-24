
namespace PokerOfflineClient;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();

    }

   

    public override void CloseWindow(Window window)
    {
        base.CloseWindow(window);
    }
}
