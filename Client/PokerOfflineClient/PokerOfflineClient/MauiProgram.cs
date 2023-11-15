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


		

       // builder.Services.AddSingleton<IApiClient>(new ApiClient("http://192.168.0.126:8080"));
        builder.Services.AddSingleton<IApiClient>(new ApiClient("http://192.168.0.126:8080"));

        builder.Services.AddTransient<RoomPage>();
        builder.Services.AddTransient<RoomViewModel>();


        builder.Services.AddTransient<ListRoomsPage>();
        builder.Services.AddTransient<ListRoomsViewModel>();

        /*#if DEBUG
		builder.Logging.AddDebug();
#endif*/

        return builder.Build();
	}
}
