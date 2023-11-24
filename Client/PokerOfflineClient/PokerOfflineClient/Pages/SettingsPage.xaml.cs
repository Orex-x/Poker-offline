using PokerOfflineClient.ViewModels;

namespace PokerOfflineClient.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}