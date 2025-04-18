using MealMe.ViewModels;

namespace MealMe.Views;

public partial class MealPlan : ContentPage
{
    public MealPlan(MealPlanVM mealPlanVM, UISettingsVM uiSettingsVM)
	{
		InitializeComponent();
        BindingContext = mealPlanVM;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        MealPlanVM mealPlanVM = (MealPlanVM)BindingContext;
        DateTime today = DateTime.Now;
        await mealPlanVM.GetMealPlans(today);
        await mealPlanVM.SetColour();
    }
}