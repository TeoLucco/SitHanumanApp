using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using SitHanumanApp.Services;
using System;
using System.IO;

namespace SitHanumanApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Determina l'ambiente di esecuzione
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
            var configFileName = $"appsettings.{environment}.json";
            
            // Configura il file di configurazione
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFileName, optional: true, reloadOnChange: true)
                .Build();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Registrazione dei servizi
            builder.Services.AddSingleton<IConfiguration>(configuration);
            builder.Services.AddSingleton<TokenService>();

            // Configura il logger se in debug mode
#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Costruisci l'app
            return builder.Build();
        }
    }

}
