// MauiProgram.cs
using Microsoft.Extensions.Logging;
using UBSFacil;
using UBSFacil.Services; // ⬅️ Namespace do seu DatabaseService
using UBSFacil.View;    // ⬅️ Namespace das suas Pages

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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Registrar Serviços
        builder.Services.AddSingleton<DatabaseService>();

        // Registrar Páginas para Injeção de Dependência
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<CadastroPage>();
        builder.Services.AddTransient<HomePage>();

        return builder.Build();
    }
}