using BeFit.Mobile.ViewModels;

namespace BeFit.Mobile.Views;

public partial class TrainingSessionsPage : ContentPage
{
    public TrainingSessionsPage(TrainingSessionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TrainingSessionsViewModel vm)
        {
            vm.LoadTrainingSessionsCommand.Execute(null);
        }
    }
}
