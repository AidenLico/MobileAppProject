using MealMe.ViewModels;

namespace MealMe.Views;

public partial class UserSettings : ContentPage
{
	public UserSettings(UserSettingsVM userSettingsVM)
	{
		InitializeComponent();
		BindingContext = userSettingsVM;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        UserSettingsVM userSettingsVM = (UserSettingsVM)BindingContext;
        await userSettingsVM.SetColour();
        await userSettingsVM.GetEmail();
    }
}