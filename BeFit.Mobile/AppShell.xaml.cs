using BeFit.Mobile.Views;

namespace BeFit.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(ExerciseTypeDetailPage), typeof(ExerciseTypeDetailPage));
        Routing.RegisterRoute(nameof(TrainingSessionDetailPage), typeof(TrainingSessionDetailPage));
        Routing.RegisterRoute(nameof(TrainingEntryDetailPage), typeof(TrainingEntryDetailPage));
    }
}
