using BeFit.Mobile.ViewModels;

namespace BeFit.Mobile.Views;

public partial class ExerciseTypesPage : ContentPage
{
    public ExerciseTypesPage(ExerciseTypesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ExerciseTypesViewModel vm)
        {
            vm.LoadExerciseTypesCommand.Execute(null);
        }
    }
}
