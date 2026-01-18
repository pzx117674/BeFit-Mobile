using BeFit.Mobile.ViewModels;

namespace BeFit.Mobile.Views;

public partial class ExerciseTypeDetailPage : ContentPage
{
    public ExerciseTypeDetailPage(ExerciseTypeDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
