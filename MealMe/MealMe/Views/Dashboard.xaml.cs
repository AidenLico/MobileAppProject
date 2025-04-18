using MealMe.ViewModels;

namespace MealMe.Views;

public partial class Dashboard : ContentPage
{
	public Dashboard(DashboardVM dashboardVM)
	{
		InitializeComponent();
        BindingContext = dashboardVM;
    }

    // Overriding base OnAppearing Method to redirect user from dashboard
    // Until they are authenticated
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var idToken = Preferences.Get("idToken", null);

        if (idToken == null)
        {
            await Shell.Current.GoToAsync("//Login");
        }

        DashboardVM dashboardVM = (DashboardVM)BindingContext;
        await dashboardVM.SetColour();
        await dashboardVM.GetTodayMeals();
    }
}