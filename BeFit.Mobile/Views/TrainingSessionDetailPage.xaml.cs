using BeFit.Mobile.ViewModels;

namespace BeFit.Mobile.Views;

public partial class TrainingSessionDetailPage : ContentPage
{
    public TrainingSessionDetailPage(TrainingSessionDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
