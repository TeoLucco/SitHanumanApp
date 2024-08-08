using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using SitHanumanApp.Services;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace SitHanumanApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.AddAppSettings();

            // Registrazione dei servizi
            builder.Services.AddSingleton<TokenService>();

            // Configura il logger se in debug mode
#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Costruisci l'app
            return builder.Build();
        }
    
        private static void AddAppSettings(this MauiAppBuilder builder)
        {
            builder.AddJsonSettings("appsettings.json");
#if DEBUG
            builder.AddJsonSettings("appsettings.development.json");
#endif
#if !DEBUG
            builder.AddJsonSettings("appsettings.production.json");
#endif
        }

        private static void AddJsonSettings(this MauiAppBuilder builder, string filename)
        {
            using Stream stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream($"SitHanumanApp.{filename}");

            if (stream != null)
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();
                builder.Configuration.AddConfiguration(config);
            }
        }
    }

}
