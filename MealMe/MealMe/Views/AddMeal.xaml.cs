using MealMe.ViewModels;

namespace MealMe.Views;

public partial class AddMeal : ContentPage
{
	public AddMeal(AddMealVM addMealVM)
	{
		InitializeComponent();
		BindingContext = addMealVM;
	}
    // Overrides base On appearing to add execution when the oage appears \\
    protected override async void OnAppearing()
    {
        // Include the base on appearing stuff \\
        base.OnAppearing();
        // Create a Add viewmodel instance as the binding context view model \\
        AddMealVM addMealVM = (AddMealVM)BindingContext;
        // Set Colour \\
        await addMealVM.SetColour();
    }
}