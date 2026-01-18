using BeFit.Mobile.Services;
using BeFit.Mobile.ViewModels;
using BeFit.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace BeFit.Mobile;

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

        builder.Services.AddSingleton<DatabaseService>();

        builder.Services.AddTransient<ExerciseTypesViewModel>();
        builder.Services.AddTransient<ExerciseTypeDetailViewModel>();
        builder.Services.AddTransient<TrainingSessionsViewModel>();
        builder.Services.AddTransient<TrainingSessionDetailViewModel>();
        builder.Services.AddTransient<TrainingEntriesViewModel>();
        builder.Services.AddTransient<TrainingEntryDetailViewModel>();

        builder.Services.AddTransient<ExerciseTypesPage>();
        builder.Services.AddTransient<ExerciseTypeDetailPage>();
        builder.Services.AddTransient<TrainingSessionsPage>();
        builder.Services.AddTransient<TrainingSessionDetailPage>();
        builder.Services.AddTransient<TrainingEntriesPage>();
        builder.Services.AddTransient<TrainingEntryDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
