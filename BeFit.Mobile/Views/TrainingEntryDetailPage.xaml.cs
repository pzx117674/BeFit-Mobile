using BeFit.Mobile.ViewModels;

namespace BeFit.Mobile.Views;

public partial class TrainingEntryDetailPage : ContentPage
{
    public TrainingEntryDetailPage(TrainingEntryDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.LoadDataCommand.Execute(null);
    }
}
