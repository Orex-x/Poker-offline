using Microsoft.Maui.LifecycleEvents;
using PokerOffline.Services.ApiClient;
using PokerOfflineClient.Pages;
using PokerOfflineClient.ViewModels;

namespace PokerOfflineClient;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        var baseUrl = SecureStorage.GetAsync("base_url").Result;

        if (string.IsNullOrEmpty(baseUrl))
        {
            baseUrl = "http://192.168.0.1";
            SecureStorage.Default.SetAsync("base_url", baseUrl);
        }

        builder.Services.AddSingleton<IApiClient>(new ApiClient(baseUrl));
        
        builder.Services.AddTransient<RoomPage>();
        builder.Services.AddTransient<RoomViewModel>();

        builder.Services.AddTransient<CreateRoomPage>();
        builder.Services.AddTransient<CreateRoomViewModel>();

        builder.Services.AddTransient<ListRoomsPage>();
        builder.Services.AddTransient<ListRoomsViewModel>();

        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<SettingsViewModel>();

        return builder.Build();
	}
}
