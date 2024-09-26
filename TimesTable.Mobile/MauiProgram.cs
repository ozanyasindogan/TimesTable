using TimesTable.Mobile.Services;

namespace TimesTable.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
    {
        var turkishCulture = new CultureInfo("tr-TR", false);
		CultureInfo.CurrentCulture = turkishCulture;
		CultureInfo.CurrentUICulture = turkishCulture;
        Thread.CurrentThread.CurrentCulture = turkishCulture;
		Thread.CurrentThread.CurrentUICulture = turkishCulture;
		
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<MainViewModel>();

		builder.Services.AddSingleton<MainPage>();

		builder.Services.AddSingleton<GameViewModel>();

		builder.Services.AddSingleton<GamePage>();

		builder.Services.AddSingleton<ResultsViewModel>();

		builder.Services.AddSingleton<ResultsPage>();

		builder.Services.AddSingleton<DatabaseService>();

		builder.Services.AddSingleton(AudioManager.Current);

		return builder.Build();
	}
}
