using MauiApp2.Data;
using MauiApp2.ViewModel;
using Microsoft.Extensions.Logging;
using Shiny;
using CommunityToolkit.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace MauiApp2
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseSkiaSharp(true)
                    .UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkitMediaElement().UseMauiCommunityToolkit();
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "revmetrix.db");
            builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<UserRepository>(s, dbPath));
#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddBluetoothLE();
            return builder.Build();
        }
    }
}