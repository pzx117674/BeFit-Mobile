using BeFit.Mobile.ViewModels;

namespace BeFit.Mobile.Views;

public partial class TrainingEntriesPage : ContentPage
{
    public TrainingEntriesPage(TrainingEntriesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TrainingEntriesViewModel vm)
        {
            vm.LoadTrainingEntriesCommand.Execute(null);
        }
    }
}
