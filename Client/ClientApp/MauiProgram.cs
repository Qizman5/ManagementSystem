using Microsoft.Extensions.Logging;
using ClientApp.ViewModels;
using ClientApp.Views;
using System.Net;

namespace ClientApp;

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

        // Єдина налаштована реєстрація HttpClient
        builder.Services.AddSingleton(sp => 
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true,
                AllowAutoRedirect = false,
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri("http://127.0.0.1:5024/")
            };
        });

        // Реєстрація ViewModels та сторінок
        builder.Services.AddTransient<ItemsViewModel>();
        builder.Services.AddTransient<ItemsPage>();

        builder.Services.AddTransient<CreateOperationViewModel>();
        builder.Services.AddTransient<CreateOperationPage>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
