using Microsoft.Extensions.Logging;
using Samsung_TV_Remote_Automation.Services;
using Samsung_TV_Remote_Automation.ViewModels;
using Samsung_TV_Remote_Automation.Views;

namespace Samsung_TV_Remote_Automation
{
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

        //Register SamsungTvClient as a singleton service (an instance under the apps lifetime)
        builder.Services.AddSingleton<SamsungTvClient>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
}